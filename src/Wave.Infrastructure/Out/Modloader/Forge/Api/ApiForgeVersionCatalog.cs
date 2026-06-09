using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.Serialization;
using Wave.Application.Out.Modloader;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;
using Wave.Infrastructure.Out.Modloader.Forge.Api.Dtos;

namespace Wave.Infrastructure.Out.Modloader.Forge.Api;

public class ApiForgeVersionCatalog : IModloaderVersionCatalog
{
    private static readonly HttpClient client = new();

    public ModloaderType ModloaderType { get; private set; } = ModloaderType.Forge;

    public async Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(string minecraftVersion, CancellationToken ct = default)
    {
        List<ModloaderInfo> forgeVersions = new List<ModloaderInfo>();
        try
        {
            string xmlResponse = await client.GetStringAsync("https://maven.minecraftforge.net/net/minecraftforge/forge/maven-metadata.xml", ct);

            XDocument doc = XDocument.Parse(xmlResponse);

            XmlSerializer serializer = new XmlSerializer(typeof(Metadata));
            Metadata metadata = (Metadata)serializer.Deserialize(doc.CreateReader())!;

            List<string> versionBundles = metadata.Versioning?.Versions ?? new List<string>();



            foreach (string versionBundle in versionBundles)
            {
                forgeVersions.Add(Mapper.ToDomain(versionBundle));
            }

            forgeVersions = forgeVersions.Where(f => minecraftVersion == f.MinecraftVersion).ToList();
            forgeVersions.Reverse();
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No Forge versions were found.");
        }

        return forgeVersions;
    }

    public async Task<ModloaderPackage> DownloadModloaderAsync(ModloaderInfo modloaderInfo, string filePath, CancellationToken ct = default)
    {
        //Download latest installer

        string mcVersion = modloaderInfo.MinecraftVersion;
        string forgeVersion = modloaderInfo.Version;
        using (
            var response = await client.GetAsync(
                $"https://maven.minecraftforge.net/net/minecraftforge/forge/{mcVersion}-{forgeVersion}/forge-{mcVersion}-{forgeVersion}-installer.jar",
                HttpCompletionOption.ResponseHeadersRead
                )
        )
        {
            response.EnsureSuccessStatusCode();

            using (var fileStream = File.Create(filePath))
            {
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    await httpStream.CopyToAsync(fileStream);
                }
            }
        }

        if (!File.Exists(filePath)) throw new IOException("Nothing was downloaded.");

        return new ModloaderPackage()
        {
            ModloaderType = ModloaderType,
            InstallerPath = filePath,
            InstallerVersion = "latest",
            Version = modloaderInfo.Version,
            MinecraftVersion = modloaderInfo.MinecraftVersion
        };
    }

    public async Task<ModloaderInstallation> InstallModloaderAsync(string targetDirectory, ModloaderPackage modloaderPackage, JavaInstallation javaInstallation, CancellationToken ct = default)
    {
        if (!Directory.Exists(targetDirectory)) throw new IOException("Target directory does not exist.");
        if (!File.Exists(modloaderPackage.InstallerPath)) throw new IOException($"File '{modloaderPackage.InstallerPath}' does not exist.");

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = javaInstallation.ExecutableFile,
                WorkingDirectory = Path.GetDirectoryName(modloaderPackage.InstallerPath),

                Arguments = $"-jar \"{modloaderPackage.InstallerPath}\" -installServer {targetDirectory}",
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        var tcs = new TaskCompletionSource<int>();

        process.Exited += (sender, args) =>
        {
            tcs.SetResult(process.ExitCode);
            process.Dispose();
        };

        process.Start();

        int result = await tcs.Task;

        File.Delete(modloaderPackage.InstallerPath);

        if (result != 0) throw new Exception($"Fabric installation failed. Exited with code {result}"); //TODO: Mejorar handling de la excepción.

        return new ModloaderInstallation()
        {
            ModloaderType = ModloaderType,
            MinecraftVersion = modloaderPackage.MinecraftVersion,
            Version = modloaderPackage.Version
        };
    }

    public bool CanHandleType(ModloaderType type)
    {
        return type == ModloaderType;
    }
}

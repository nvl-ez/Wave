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

    public async Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(MinecraftVersionInfo minecraftVersionInfo, CancellationToken ct = default)
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
                forgeVersions.Add(Mapper.ToDomain(versionBundle, minecraftVersionInfo));
            }

            forgeVersions = forgeVersions.Where(f => minecraftVersionInfo.MinecraftVersion == f.MinecraftVersion).ToList();
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No Forge versions were found.");
        }

        return forgeVersions;
    }

    public async Task<ModloaderPackage> DownloadModloaderAsync(ModloaderInfo modloaderInfo, string path, CancellationToken ct = default)
    {
        //Download latest installer
        string filePath = "";

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

            string fileName = response.Content.Headers.ContentDisposition!.FileName!;

            filePath = Path.Combine(path, fileName);
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
            ModloaderType = ModloaderType.Fabric,
            InstallerPath = filePath,
            InstallerVersion = "latest",
            ModloaderVersion = modloaderInfo.Version,
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
                WorkingDirectory = Path.GetFullPath(modloaderPackage.InstallerPath),

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

        await tcs.Task;

        File.Delete(modloaderPackage.InstallerPath);

        if (process.ExitCode != 0) throw new Exception($"Fabric installation failed. Exited with code {process.ExitCode}"); //TODO: Mejorar handling de la excepción.

        return new ModloaderInstallation()
        {
            Type = ModloaderType.Fabric,
            MinecraftVersion = modloaderPackage.MinecraftVersion,
            Version = modloaderPackage.InstallerVersion
        };
    }

    public bool CanHandleType(ModloaderType type)
    {
        return type == ModloaderType.Forge;
    }
}

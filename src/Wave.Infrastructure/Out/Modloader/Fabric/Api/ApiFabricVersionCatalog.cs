using System;
using System.Diagnostics;
using System.Text.Json;
using Wave.Application.Out.Modloader;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;
using Wave.Infrastructure.Out.Modloader.Fabric.Api.Dtos;

namespace Wave.Infrastructure.Out.Modloader.Fabric.Api;

public class ApiFabricVersionCatalog : IModloaderVersionCatalog
{
    private static readonly HttpClient client = new();

    public ModloaderType ModloaderType { get; private set; } = ModloaderType.Fabric;

    public async Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(string minecraftVersion, CancellationToken ct = default)
    {
        List<ModloaderInfo> fabricVersions = new List<ModloaderInfo>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"https://meta.fabricmc.net/v2/versions/loader/{minecraftVersion}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement versionsElement = doc.RootElement;
            List<FabricVersionJsonDto> dtoVersions = JsonSerializer.Deserialize<List<FabricVersionJsonDto>>(versionsElement) ?? new List<FabricVersionJsonDto>();
            foreach (FabricVersionJsonDto dtoVersion in dtoVersions)
            {
                fabricVersions.Add(Mapper.ToDomain(dtoVersion, minecraftVersion));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No Fabric versions were found.");
        }

        return fabricVersions;
    }

    public async Task<ModloaderPackage> DownloadModloaderAsync(ModloaderInfo modloaderInfo, string filePath, CancellationToken ct = default)
    {

        string jsonResponse = await client.GetStringAsync("https://meta.fabricmc.net/v2/versions/installer", ct);
        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement versionsElement = doc.RootElement;
        List<InstallerInfoDto> installerInfos = JsonSerializer.Deserialize<List<InstallerInfoDto>>(versionsElement) ?? new List<InstallerInfoDto>();

        InstallerInfoDto latest = installerInfos.First();

        //Download latest installer
        using (var response = await client.GetAsync(latest.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
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
            InstallerVersion = latest.DownloadUrl,
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

                Arguments =
                $"-jar \"{modloaderPackage.InstallerPath}\" server " +
                $"-dir \"{targetDirectory}\" " +
                $"-mcversion {modloaderPackage.MinecraftVersion} " +
                $"-loader {modloaderPackage.Version} " +
                "-noprofile",
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
            Version = modloaderPackage.InstallerVersion
        };
    }

    public bool CanHandleType(ModloaderType type)
    {
        return type == ModloaderType;
    }
}

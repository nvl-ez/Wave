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

    public async Task<ModloaderPackage> DownloadModloaderAsync(ModloaderInfo modloaderInfo, string path, CancellationToken ct = default)
    {

        string jsonResponse = await client.GetStringAsync("https://meta.fabricmc.net/v2/versions/installer", ct);
        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement versionsElement = doc.RootElement;
        Dictionary<int, InstallerInfoDto> installerInfos = JsonSerializer.Deserialize<Dictionary<int, InstallerInfoDto>>(versionsElement) ?? new Dictionary<int, InstallerInfoDto>();

        InstallerInfoDto latest = installerInfos.OrderBy(i => i.Key).Last(i => i.Value.Stable == true).Value;

        //Download latest installer
        string filePath = "";
        using (var response = await client.GetAsync(latest.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
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
            ModloaderType = ModloaderType,
            InstallerPath = filePath,
            InstallerVersion = latest.DownloadUrl,
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

                Arguments = $"-jar \"{modloaderPackage.InstallerPath}\" -dir \"{targetDirectory}\" -mcversion {modloaderPackage.MinecraftVersion} -noprofile -snapshot -loader {modloaderPackage.ModloaderVersion}",
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
            Type = ModloaderType,
            MinecraftVersion = modloaderPackage.MinecraftVersion,
            Version = modloaderPackage.InstallerVersion
        };
    }

    public bool CanHandleType(ModloaderType type)
    {
        return type == ModloaderType;
    }
}

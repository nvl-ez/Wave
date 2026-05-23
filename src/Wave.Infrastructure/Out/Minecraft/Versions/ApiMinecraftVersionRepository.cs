using System;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using Wave.Application.Out.Minecraft;
using Wave.Domain.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Versions.MinecraftVersionDtos;
using Wave.Infrastructure.Out.Minecraft.Versions.VersionDetailDtos;

namespace Wave.Infrastructure.Out.Minecraft.Api;

public class ApiMinecraftVersionRepository : IMinecraftVersionRepository
{
    private static HttpClient client = new();

    //Download and returns the java version required for the server to run
    public async Task<MinecraftVersionInstallation> DownloadMinecraftServer(MinecraftVersionDetails minecraftVersionDetails, string serverJarPath, CancellationToken ct = default)
    {
        string jarEndpoint = minecraftVersionDetails.ServerUrl;
        if (string.IsNullOrEmpty(jarEndpoint)) throw new InvalidDataException("Server download url cannot be null or empty");

        using var downloadStream = await client.GetStreamAsync(jarEndpoint);
        using var fileStream = new FileStream(serverJarPath, FileMode.Create);

        await downloadStream.CopyToAsync(fileStream);
        await fileStream.FlushAsync();
        fileStream.Close();

        return new MinecraftVersionInstallation()
        {
            JavaVersion = minecraftVersionDetails.JavaVersion,
            MinecraftVersion = minecraftVersionDetails.MinecraftVersion,
            MinecraftVersionType = minecraftVersionDetails.MinecraftVersionType,
            ReleaseDate = minecraftVersionDetails.ReleaseDate
        };
    }

    public async Task<MinecraftVersionDetails> GetVersionDetailsAsync(MinecraftVersionInfo minecraftVersionInfo, CancellationToken ct = default)
    {
        string jsonResponse = await client.GetStringAsync(minecraftVersionInfo.DetailsUrl, ct);
        JsonDocument doc = JsonDocument.Parse(jsonResponse);

        VersionDetailDto? dto = JsonSerializer.Deserialize<VersionDetailDto>(doc.RootElement) ?? null;

        if (dto is null) throw new SerializationException("Server details DTO could not be deserialized.");

        return new MinecraftVersionDetails()
        {
            JavaVersion = dto.JavaVersion.MajorVersion,
            ServerUrl = dto.Downloads.Server.Url,
            MinecraftVersion = minecraftVersionInfo.MinecraftVersion,
            MinecraftVersionType = minecraftVersionInfo.MinecraftVersionType,
            ReleaseDate = minecraftVersionInfo.ReleaseDate
        };
    }

    public async Task<List<MinecraftVersionInfo>> GetAllVersionsAsync(CancellationToken ct = default)
    {
        string jsonResponse = await client.GetStringAsync("https://launchermeta.mojang.com/mc/game/version_manifest.json", ct);

        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement versionsElement = doc.RootElement.GetProperty("versions");

        List<MinecraftVersionJson>? dtoVersions = JsonSerializer.Deserialize<List<MinecraftVersionJson>>(versionsElement) ?? null;

        if (dtoVersions is null) throw new SerializationException("Server DTO could not be deserialized.");

        List<MinecraftVersionInfo> versions = new List<MinecraftVersionInfo>();

        foreach (MinecraftVersionJson dtoVersion in dtoVersions)
        {
            versions.Add(Mapper.ToDomain(dtoVersion));
        }

        return versions;
    }
}

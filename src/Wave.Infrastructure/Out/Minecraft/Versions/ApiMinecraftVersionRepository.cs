using System;
using System.Net.Http.Json;
using System.Text.Json;
using Wave.Application.Out.Minecraft;
using Wave.Domain.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Versions.Dtos;

namespace Wave.Infrastructure.Out.Minecraft.Api;

public class ApiMinecraftVersionRepository : IMinecraftVersionRepository
{
    private static HttpClient client = new()
    {
        BaseAddress = new Uri("https://launchermeta.mojang.com/mc/game/version_manifest.json")
    };

    public async Task<List<MinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken ct = default)
    {
        string jsonResponse = await client.GetStringAsync("", ct);

        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement versionsElement = doc.RootElement.GetProperty("versions");

        List<MinecraftVersionJson> dtoVersions = JsonSerializer.Deserialize<List<MinecraftVersionJson>>(versionsElement) ?? new List<MinecraftVersionJson>();

        List<MinecraftVersion> versions = new List<MinecraftVersion>();

        foreach (MinecraftVersionJson dtoVersion in dtoVersions)
        {
            versions.Add(Mapper.ToDomain(dtoVersion));
        }

        return versions;
    }
}

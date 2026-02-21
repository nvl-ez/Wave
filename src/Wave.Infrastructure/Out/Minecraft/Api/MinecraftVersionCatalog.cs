using System;
using System.Net.Http.Json;
using System.Text.Json;
using Wave.Application.Out.Minecraft.Api;
using Wave.Domain.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Api.Dtos;
using Wave.Infrastructure.Out.Minecraft.Api.Mappers;

namespace Wave.Infrastructure.Out.Minecraft.Api;

public class MinecraftVersionCatalog : IMinecraftVersionCatalog
{
    private static HttpClient client = new()
    {
        BaseAddress = new Uri("https://launchermeta.mojang.com/mc/game/version_manifest.json")
    };

    public async Task<List<MinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken ct)
    {
        string jsonResponse = await client.GetStringAsync("", ct);

        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement versionsElement = doc.RootElement.GetProperty("versions");

        List<MinecraftVersionJson> dtoVersions = JsonSerializer.Deserialize<List<MinecraftVersionJson>>(versionsElement) ?? new List<MinecraftVersionJson>();

        List<MinecraftVersion> versions = new List<MinecraftVersion>();

        foreach (MinecraftVersionJson dtoVersion in dtoVersions)
        {
            versions.Add(MinecraftVersionJsonMapper.ToDomain(dtoVersion));
        }

        return versions;
    }
}

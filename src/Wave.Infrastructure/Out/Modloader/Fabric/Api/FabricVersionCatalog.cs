using System;
using System.Text.Json;
using Wave.Application.Out.Modloader.Api;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Infrastructure.Out.Modloader.Fabric.Api.Dtos;
using Wave.Infrastructure.Out.Modloader.Fabric.Api.Mappers;

namespace Wave.Infrastructure.Out.Modloader.Fabric.Api;

public class FabricVersionCatalog : IModloaderVersionCatalog
{
    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("https://meta.fabricmc.net/v2/versions/loader/")
    };

    public async Task<IEnumerable<ModloaderVersion>> GetModloaderVersionsAsync(MinecraftVersion minecraftVersion, CancellationToken ct)
    {
        List<FabricVersion> fabricVersions = new List<FabricVersion>();
        try
        {
            string jsonResponse = await client.GetStringAsync(minecraftVersion.Version, ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement versionsElement = doc.RootElement;
            List<FabricVersionJson> dtoVersions = JsonSerializer.Deserialize<List<FabricVersionJson>>(versionsElement) ?? new List<FabricVersionJson>();
            foreach (FabricVersionJson dtoVersion in dtoVersions)
            {
                fabricVersions.Add(FabricVersionJsonMapper.ToDomain(dtoVersion, minecraftVersion.Version));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No Fabric versions were found.");
        }

        return fabricVersions;
    }
}

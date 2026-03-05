using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Infrastructure.Out.Java.Mojang.Dtos;
using Wave.Infrastructure.Out.Java.Mojang.Mappers;

namespace Wave.Infrastructure.Out.Java.Mojang;

public class MojangJavaSupplier : IJavaSupplier
{
    private readonly HttpClient client;

    public MojangJavaSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://launchermeta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json")
        };
    }

    public async Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery query, CancellationToken ct)
    {

        string jsonResponse = await client.GetStringAsync("", ct);

        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement rootElement = doc.RootElement;

        var dto = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<ReleaseDto>>>>(rootElement) ?? new Dictionary<string, Dictionary<string, List<ReleaseDto>>>();

        List<JavaVersion> versions = new List<JavaVersion>();

        if (dto is null) return versions;

        //Parse items
        foreach (KeyValuePair<string, Dictionary<string, List<ReleaseDto>>> platform in dto)
        {
            foreach (KeyValuePair<string, List<ReleaseDto>> release in platform.Value)
                if (release.Key != "gamecore" && release.Value.Count > 0)
                    versions.Add(Mapper.ToDomain(platform.Key, release.Key, release.Value.First()));
        }

        //Filter items
        versions = versions.Where(v =>
            (query.Version is null || (v.Version == query.Version)) &&
            (query.ArchitectureBitType == v.ArchitectureBitType) &&
            (query.ArchitectureType == v.ArchitectureType) &&
            (query.OsType == v.OsType)
        ).ToList();

        return versions;
    }
}

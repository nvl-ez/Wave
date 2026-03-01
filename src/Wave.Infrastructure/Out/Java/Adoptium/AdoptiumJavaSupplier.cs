using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.Os;
using Wave.Infrastructure.Out.Java.Adoptium.Dtos;
using Wave.Infrastructure.Out.Java.Adoptium.Mappers;

namespace Wave.Infrastructure.Out.Java.Adoptium;

public class AdoptiumJavaSupplier : IJavaSupplier
{
    private readonly HttpClient client;

    public AdoptiumJavaSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.adoptium.net")
        };
    }

    public async Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery? query, CancellationToken ct)
    {
        List<int> versionsToCheck = new List<int>();
        if (query is null || query.Version is null)
        {
            FeatureVersionsDto? versions = await GetFeatureVersions(ct);
            if (versions is not null)
                versionsToCheck.AddRange(versions.AvailableLtsReleases);
        }
        else
        {
            versionsToCheck.Add((int)query.Version);
        }

        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        queryParameters.Add("image_type", "jre");
        if (query is not null && query.ArchitectureType is not null)
            queryParameters.Add("architecture", Mapper.ToDtoArchitectureType((ArchitectureType)query.ArchitectureType));
        if (query is not null && query.OsType is not null)
            queryParameters.Add("os", Mapper.ToDtoOsType((OsType)query.OsType));

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<JavaVersion> retrievedVersions = new List<JavaVersion>();
        foreach (int version in versionsToCheck)
        {
            try
            {
                string jsonResponse = await client.GetStringAsync($"/v3/assets/latest/{version}/hotspot?{queryString}", ct);
                JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement rootElement = doc.RootElement;

                List<LatestAssetDto> dto = JsonSerializer.Deserialize<List<LatestAssetDto>>(rootElement) ?? new List<LatestAssetDto>();
                foreach (LatestAssetDto asset in dto)
                {
                    if (asset.Binary.Installer is not null || asset.Binary.Package is not null)
                        retrievedVersions.Add(Mapper.ToDomain(asset));
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Error when contacting Adoptium");
            }
        }
        return retrievedVersions;
    }

    private async Task<FeatureVersionsDto?> GetFeatureVersions(CancellationToken ct)
    {
        try
        {
            string jsonResponse = await client.GetStringAsync("/v3/info/available_releases", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            FeatureVersionsDto? dto = JsonSerializer.Deserialize<FeatureVersionsDto>(rootElement);

            return dto;
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Adoptium");
        }
        return null;
    }
}

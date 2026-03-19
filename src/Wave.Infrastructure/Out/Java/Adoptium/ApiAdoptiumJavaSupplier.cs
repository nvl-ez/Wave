using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.Os;
using Wave.Infrastructure.Out.Java.Adoptium.Dtos;

namespace Wave.Infrastructure.Out.Java.Adoptium;

public class ApiAdoptiumJavaSupplier : IJavaSupplier
{
    private readonly HttpClient client;

    public ApiAdoptiumJavaSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.adoptium.net")
        };
    }

    public async Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery query, CancellationToken ct = default)
    {
        List<int> versionsToCheck = new List<int>();
        if (query.Version is null)
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
        queryParameters.Add("architecture", Mapper.ToDtoArchitectureType(query.ArchitectureType));
        queryParameters.Add("os", Mapper.ToDtoOsType(query.OsType));

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<JavaVersion> retrievedVersions = new List<JavaVersion>();
        foreach (int version in versionsToCheck)
        {
            try
            {
                string jsonResponse = await client.GetStringAsync($"/v3/assets/feature_releases/{version}/ga?{queryString}", ct);
                JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement rootElement = doc.RootElement;

                List<BuildsDto> dto = JsonSerializer.Deserialize<List<BuildsDto>>(rootElement) ?? new List<BuildsDto>();
                foreach (BuildsDto build in dto)
                {
                    BinaryDto binary = build.Binaries.First();
                    if (binary.Installer is not null || binary.Package is not null)
                        retrievedVersions.Add(Mapper.ToDomain(build, binary));
                }
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Error when contacting Adoptium");
            }
        }
        return retrievedVersions;
    }

    private async Task<FeatureVersionsDto?> GetFeatureVersions(CancellationToken ct = default)
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

using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.System;
using Wave.Infrastructure.Out.Java.Adoptium.Dtos;
using Wave.Infrastructure.Out.Java.JavaPackage;

namespace Wave.Infrastructure.Out.Java.Adoptium;

public class ApiAdoptiumJavaSupplier : IJavaSupplier
{
    private readonly HttpClient client;
    private readonly string javaTmpDirectory;

    public ApiAdoptiumJavaSupplier(string javaTmpDirectory)
    {
        client = new HttpClient();
        this.javaTmpDirectory = javaTmpDirectory;
    }

    public string Name { get; set; } = "Adoptium";

    public bool CanDownload(JavaVersion javaVersion)
    {
        return javaVersion.JavaSupplierType == JavaSupplierType.Adoptium;
    }

    public async Task<IJavaPackage> DownloadJavaAsync(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct = default)
    {
        string fileName = "";
        using (var response = await client.GetAsync(javaArtifact.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();

            fileName = response.Content.Headers.ContentDisposition!.FileName!;

            string filePath = Path.Combine(javaTmpDirectory, fileName);
            using (var fileStream = File.Create(filePath))
            {
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    await httpStream.CopyToAsync(fileStream);
                }
            }
        }

        string packagePath = Path.Combine(javaTmpDirectory, fileName);

        if (!File.Exists(packagePath) && !Directory.Exists(packagePath)) throw new IOException("Nothing was downloaded.");

        switch (javaArtifact.Type)
        {
            case JavaArtifactType.Compressed:
                return new CompressedJavaPackage()
                {
                    Filename = fileName,
                    PackageDirectory = javaTmpDirectory,
                    JavaSupplierType = JavaSupplierType.Adoptium,
                    JavaName = javaVersion.Name,
                    Version = javaVersion.Version,
                    JavaArtifactType = JavaArtifactType.Compressed
                };
            case JavaArtifactType.Manifest:
                throw new NotSupportedException("Adoptium does not support Manifest packages.");
            default:
                throw new NotImplementedException($"The type {javaArtifact.Type} doesn't have package implementation.");
        }
    }

    public async Task<IEnumerable<int>> GetAvailableMajorVersionsAsync(OsType? os = null, CancellationToken ct = default)
    {
        string jsonResponse = await client.GetStringAsync("https://api.adoptium.net/v3/info/available_releases", ct);
        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement rootElement = doc.RootElement;

        FeatureVersionsDto? dto = JsonSerializer.Deserialize<FeatureVersionsDto>(rootElement);

        return dto!.AvailableLtsReleases;
    }

    public async Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery query, CancellationToken ct = default)
    {
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        queryParameters.Add("image_type", "jre");
        queryParameters.Add("architecture", Mapper.ToDtoArchitectureType(query.ArchitectureType));
        queryParameters.Add("os", Mapper.ToDtoOsType(query.OsType));

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<JavaVersion> retrievedVersions = new List<JavaVersion>();


        string jsonResponse = await client.GetStringAsync($"https://api.adoptium.net/v3/assets/feature_releases/{query.Version}/ga?{queryString}", ct);
        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement rootElement = doc.RootElement;

        List<BuildsDto> dto = JsonSerializer.Deserialize<List<BuildsDto>>(rootElement) ?? new List<BuildsDto>();
        foreach (BuildsDto build in dto)
        {
            BinaryDto binary = build.Binaries.First();
            if (binary.Package is not null)
                retrievedVersions.Add(Mapper.ToDomain(build, binary));
        }


        return retrievedVersions;
    }
}

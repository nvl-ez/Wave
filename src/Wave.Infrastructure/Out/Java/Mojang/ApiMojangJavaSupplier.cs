using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Infrastructure.Out.Java.JavaPackage;
using Wave.Infrastructure.Out.Java.Mojang.Dtos;

namespace Wave.Infrastructure.Out.Java.Mojang;

public class ApiMojangJavaSupplier : IJavaSupplier
{
    private readonly HttpClient client;
    private readonly string javaTmpDirectory;

    public ApiMojangJavaSupplier(string javaTmpDirectory)
    {
        client = new HttpClient();
        this.javaTmpDirectory = javaTmpDirectory;
    }

    public async Task<IJavaPackage> DownloadJavaAsync(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct = default)
    {
        string jsonResponse = await client.GetStringAsync(javaArtifact.DownloadUrl, ct);

        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement rootElement = doc.RootElement;

        JavaManifestDto manifest = JsonSerializer.Deserialize<JavaManifestDto>(rootElement) ?? new JavaManifestDto();

        if (manifest.Files is null) throw new NullReferenceException("Manifest contains no files.");
        var directories = manifest.Files.Where(kv => kv.Value.Type == "directory");
        var files = manifest.Files.Where(kv => kv.Value.Type == "file");
        var links = manifest.Files.Where(kv => kv.Value.Type == "link");

        //Create all directories
        string fileName = javaVersion.Name;
        string basePath = Path.Combine(javaTmpDirectory, fileName);

        foreach (var directory in directories)
        {
            string fullPath = Path.Combine(basePath, directory.Key);
            Directory.CreateDirectory(fullPath);
        }

        //Download each file in the respective directory
        foreach (var file in files)
        {
            string fullPath = Path.Combine(basePath, file.Key);

            if (file.Value?.Downloads?.Raw is null) throw new NullReferenceException("A file download has been null.");
            using var downloadStream = await client.GetStreamAsync(file.Value.Downloads.Raw.Url);
            using var fileStream = new FileStream(fullPath, FileMode.Create);

            await downloadStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileStream.Close();
        }

        return new ManifestJavaPackage()
        {
            Filename = fileName,
            PackageDirectory = javaTmpDirectory,
            JavaSupplierType = JavaSupplierType.Mojang,
            JavaName = javaVersion.Name,
            Version = javaVersion.Version
        };
    }

    public async Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery query, CancellationToken ct = default)
    {

        string jsonResponse = await client.GetStringAsync("https://launchermeta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json", ct);

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

using System;
using System.Text.Json;
using Wave.Application.In;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.Mods;
using Wave.Infrastructure.Out.Java.Installer.Dtos;

namespace Wave.Infrastructure.Out.Java.Installer;

public class ManifestInstaller : IJavaInstaller
{
    private string javaDirectory;
    private readonly HttpClient client;

    public ManifestInstaller(string javaDirectory)
    {
        this.javaDirectory = javaDirectory;
        client = new HttpClient();
    }

    public async Task<JavaInstallation?> Install(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct)
    {
        ManifestDto manifest = await GetManifestDto(javaArtifact, ct);
        if (manifest is null || manifest.Files is null)
            return null;

        string basePath = Path.Combine(javaDirectory, $"{javaVersion.JavaSupplierType}-{javaVersion.Version}");
        await DownloadFiles(javaVersion, manifest, basePath, ct);

        return GetJavaInstallation(javaVersion, manifest, basePath);
    }

    public async Task<bool> Uninstall(JavaInstallation javaInstallation, CancellationToken ct)
    {
        try
        {
            Directory.Delete(javaInstallation.UninstallerPath, true);
        }
        catch
        {
            return false;
        }
        return true;
    }

    private async Task<ManifestDto> GetManifestDto(JavaArtifact javaArtifact, CancellationToken ct)
    {
        client.BaseAddress = new Uri(javaArtifact.DownloadUrl);
        try
        {
            string jsonResponse = await client.GetStringAsync("", ct);

            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            return JsonSerializer.Deserialize<ManifestDto>(rootElement) ?? new ManifestDto();

        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Mojang");
        }
        return new ManifestDto();
    }

    private async Task DownloadFiles(JavaVersion javaVersion, ManifestDto manifest, string basePath, CancellationToken ct)
    {
        if (manifest.Files is null) throw new NullReferenceException("Manifest contains no files.");
        var directories = manifest.Files.Where(kv => kv.Value.Type == "directory");
        var files = manifest.Files.Where(kv => kv.Value.Type == "file");
        var links = manifest.Files.Where(kv => kv.Value.Type == "link");

        //Create all directories
        foreach (var directory in directories)
        {
            string fullPath = Path.Combine(basePath, directory.Key);
            Directory.CreateDirectory(fullPath);
        }

        //Download each file in the respective directory
        foreach (var file in files)
        {
            string fullPath = Path.Combine(basePath, file.Key);

            if (file.Value.Downloads is null || file.Value.Downloads.Raw is null) throw new NullReferenceException("A file download has been null.");
            using var downloadStream = await client.GetStreamAsync(file.Value.Downloads.Raw.Url);
            using var fileStream = new FileStream(fullPath, FileMode.Create);

            await downloadStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileStream.Close();
        }
    }

    private JavaInstallation GetJavaInstallation(JavaVersion javaVersion, ManifestDto manifest, string basePath)
    {
        if (manifest.Files is null) throw new NullReferenceException("Manifest contains no files.");
        var files = manifest.Files.Where(kv => kv.Value.Type == "file");

        string? javaPath = null;
        string? javawPath = null;

        foreach (var file in files)
        {
            string fullPath = Path.Combine(basePath, file.Key);
            string fileName = Path.GetFileNameWithoutExtension(fullPath);

            if (fileName == "java")
            {
                javaPath = fullPath;
            }
            else if (fileName == "javaw")
            {
                javawPath = fullPath;
            }
        }

        if (javaPath is null && javawPath is null) throw new FileNotFoundException("Java executable was not found in the installed files.");

        return new JavaInstallation()
        {
            ExecutablePath = javawPath != null ? javawPath : javaPath!,
            JavaArtifactType = JavaArtifactType.Manifest,
            JavaSupplierType = javaVersion.JavaSupplierType,
            Name = javaVersion.Name,
            UninstallerPath = basePath,
            Version = javaVersion.Version
        };
    }
}

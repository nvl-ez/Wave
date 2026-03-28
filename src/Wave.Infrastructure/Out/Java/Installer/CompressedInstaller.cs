using System;
using System.Formats.Tar;
using System.IO.Compression;
using Wave.Application.Out.Java;
using Wave.Domain.Java;

namespace Wave.Infrastructure.Out.Java.Installer;

public class CompressedInstaller : IJavaInstaller
{
    private string javaDirectory;
    private readonly HttpClient client;

    public CompressedInstaller(string javaDirectory)
    {
        this.javaDirectory = javaDirectory;
        client = new HttpClient();
    }

    public async Task<JavaInstallation?> Install(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct = default)
    {
        if (javaArtifact.Type != JavaArtifactType.Compressed) throw new NotSupportedException("Can only install Compressed artifacts.");

        string basePath = Path.Combine(javaDirectory, $"{javaVersion.JavaSupplierType}-{javaVersion.Name}-{javaVersion.Version}"); //Se deberia de abstraer
        if (Path.Exists(basePath))
            Directory.Delete(basePath, true);

        Directory.CreateDirectory(basePath);


        string tmpPath = await DownloadFile(javaArtifact, ct);
        await ExtractFiles(basePath, tmpPath, ct);
        return BuildJavaInstallation(javaVersion, basePath);
    }

    public async Task<bool> Uninstall(JavaInstallation javaInstallation, CancellationToken ct = default)
    {
        if (javaInstallation.JavaArtifactType != JavaArtifactType.Compressed) throw new NotSupportedException("Can only uninstall Compressed artifacts.");
        try
        {
            Directory.Delete(javaInstallation.UninstallerPath, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> DownloadFile(JavaArtifact javaArtifact, CancellationToken ct = default)
    {
        string filePath;
        using (var response = await client.GetAsync(javaArtifact.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            string tmpPath = Path.Combine(javaDirectory, "tmp");
            Directory.CreateDirectory(tmpPath);
            filePath = Path.Combine(tmpPath, response.Content.Headers.ContentDisposition!.FileName!);
            using (var fileStream = File.Create(filePath))
            {
                using (var httpStream = await response.Content.ReadAsStreamAsync())
                {
                    await httpStream.CopyToAsync(fileStream);
                }
            }
        }

        return filePath;
    }

    private async Task ExtractFiles(string basePath, string filePath, CancellationToken ct = default)
    {
        string fileExtension = Path.GetExtension(filePath);
        string tmpPath = Path.GetDirectoryName(filePath)!;

        try
        {
            if (fileExtension == ".zip")
            {
                await ZipFile.ExtractToDirectoryAsync(filePath, basePath);
            }
            else if (filePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase) ||
             filePath.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                await using FileStream compressedStream = File.OpenRead(filePath);
                await using GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);

                TarFile.ExtractToDirectory(gzipStream, basePath, overwriteFiles: true);
            }
            else if (filePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
            {
                string outputPath = Path.Combine(
                    basePath,
                    Path.GetFileNameWithoutExtension(filePath));

                await using FileStream compressedStream = File.OpenRead(filePath);
                await using FileStream outputStream = File.Create(outputPath);
                await using GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);

                await gzipStream.CopyToAsync(outputStream, ct);
            }
            else
            {
                throw new NotSupportedException($"Cannot decompress files of type {fileExtension}.");
            }
        }
        finally
        {
            Directory.Delete(tmpPath, true);
        }
    }

    private JavaInstallation BuildJavaInstallation(JavaVersion javaVersion, string basePath)
    {
        string? javaPath = null;
        string? javawPath = null;

        foreach (var file in Directory.EnumerateFiles(basePath, "*", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var fileExtension = Path.GetExtension(file);
            if (fileExtension == "" || string.Equals(fileExtension, ".exe", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(fileName, "java", StringComparison.OrdinalIgnoreCase))
                {
                    javaPath = file;
                }
                else if (string.Equals(fileName, "javaw", StringComparison.OrdinalIgnoreCase))
                {
                    javawPath = file;
                }
            }
        }

        if (javaPath is null && javawPath is null) throw new FileNotFoundException("Java executable was not found in the installed files.");

        return new JavaInstallation()
        {
            ExecutableFile = javawPath != null ? javawPath : javaPath!,
            JavaArtifactType = JavaArtifactType.Compressed,
            JavaSupplierType = javaVersion.JavaSupplierType,
            Name = javaVersion.Name,
            UninstallerPath = basePath,
            Version = javaVersion.Version
        };
    }
}

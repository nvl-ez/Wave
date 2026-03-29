using System;
using System.Formats.Tar;
using System.IO.Compression;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Infrastructure.Out.Java.JavaPackage;

namespace Wave.Infrastructure.Out.Java.Installer;

public class CompressedJavaInstaller : IJavaInstaller<CompressedJavaPackage>
{
    private string javaDirectory;

    public CompressedJavaInstaller(string javaDirectory)
    {
        this.javaDirectory = javaDirectory;
    }
    public bool CanInstall(IJavaPackage javaPackage)
    {
        return javaPackage.JavaArtifactType == JavaArtifactType.Compressed; //TODO: remove inheritance, add property
    }

    public bool CanUninstall(JavaInstallation javaInstallation)
    {
        return javaInstallation.JavaArtifactType == JavaArtifactType.Compressed;
    }

    public JavaInstallation Install(CompressedJavaPackage javaPackage, CancellationToken ct = default)
    {
        string destinationDir = Path.Combine(javaDirectory, javaPackage.JavaName);
        ExtractFiles(javaPackage, destinationDir);

        //Find java binary
        string? javaBinary = FindJavaBinary(destinationDir);

        if (javaBinary is null) throw new FileNotFoundException("Java executable was not found in the installed files.");

        return new JavaInstallation()
        {
            JavaSupplierType = javaPackage.JavaSupplierType,
            Name = javaPackage.JavaName,
            ExecutableFile = javaBinary,
            UninstallerPath = destinationDir,
            Version = javaPackage.Version,
            JavaArtifactType = JavaArtifactType.Compressed
        };
    }

    public JavaInstallation Install(IJavaPackage javaPackage, CancellationToken ct = default)
    {
        if (javaPackage is not CompressedJavaPackage) throw new InvalidOperationException("Compressed installer can only handle compressed packages.");
        return Install((CompressedJavaPackage)javaPackage);
    }

    public void Uninstall(JavaInstallation javaInstallation, CancellationToken ct = default)
    {
        if (javaInstallation.JavaArtifactType != JavaArtifactType.Compressed) throw new InvalidOperationException("Compressed installer can only handle compressed installations.");
        if (Directory.Exists(javaInstallation.UninstallerPath)) Directory.Delete(javaInstallation.UninstallerPath, true);
        else throw new IOException($"Java installation {javaInstallation.UninstallerPath} does not exist.");
    }

    private void ExtractFiles(CompressedJavaPackage javaPackage, string destinationDir)
    {
        string fileExtension = Path.GetExtension(javaPackage.Filename);
        string filePath = javaPackage.PackagePath;

        try
        {
            if (fileExtension == ".zip")
            {
                ZipFile.ExtractToDirectory(filePath, destinationDir);
            }
            else if (filePath.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase) ||
             filePath.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                using FileStream compressedStream = File.OpenRead(filePath);
                using GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);

                TarFile.ExtractToDirectory(gzipStream, destinationDir, overwriteFiles: true);
            }
            else if (filePath.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
            {
                string outputPath = Path.Combine(
                    destinationDir,
                    Path.GetFileNameWithoutExtension(filePath));

                using FileStream compressedStream = File.OpenRead(filePath);
                using FileStream outputStream = File.Create(outputPath);
                using GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);

                gzipStream.CopyTo(outputStream);
            }
            else
            {
                throw new NotSupportedException($"Cannot decompress files of type {fileExtension}.");
            }
        }
        finally
        {
            javaPackage.Dispose();
        }
    }

    private string? FindJavaBinary(string destinationDir)
    {
        string? javaPath = null;
        string? javawPath = null;

        foreach (string fullPath in Directory.EnumerateFiles(destinationDir, "*java*", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileNameWithoutExtension(fullPath);
            var fileExtension = Path.GetExtension(fullPath);
            if (fileExtension == "" || string.Equals(fileExtension, ".exe", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(fileName, "java", StringComparison.OrdinalIgnoreCase))
                {
                    javaPath = fullPath;
                }
                else if (string.Equals(fileName, "javaw", StringComparison.OrdinalIgnoreCase))
                {
                    javawPath = fullPath;
                }
            }
        }

        return javawPath is null ? javaPath : javawPath;
    }
}

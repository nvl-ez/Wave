using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Infrastructure.Out.Java.JavaPackage;

namespace Wave.Infrastructure.Out.Java.Installer;

public class ManifestJavaInstaller : IJavaInstaller<ManifestJavaPackage>
{
    private string javaDirectory;

    public ManifestJavaInstaller(string javaDirectory)
    {
        this.javaDirectory = javaDirectory;
    }

    public bool CanInstall(IJavaPackage javaPackage)
    {
        return javaPackage.JavaArtifactType == JavaArtifactType.Manifest;
    }
    public bool CanUninstall(JavaInstallation javaInstallation)
    {
        return javaInstallation.JavaArtifactType == JavaArtifactType.Manifest;
    }

    public JavaInstallation Install(ManifestJavaPackage javaPackage, CancellationToken ct = default)
    {
        string destinationDir = Path.Combine(javaDirectory, javaPackage.Filename);
        if (Directory.Exists(javaPackage.PackagePath))
        {
            Directory.Move(javaPackage.PackagePath, destinationDir);
            javaPackage.Dispose();
        }
        else
        {
            javaPackage.Dispose();
            throw new IOException($"The package path '{javaPackage.PackagePath}' does not exist.");
        }


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
            JavaArtifactType = JavaArtifactType.Manifest
        };
    }

    public Domain.Java.JavaInstallation Install(IJavaPackage javaPackage, CancellationToken ct = default)
    {
        if (javaPackage is not ManifestJavaPackage) throw new InvalidOperationException("Manifest installer can only handle manifest packages.");
        return Install((ManifestJavaPackage)javaPackage);
    }

    public void Uninstall(JavaInstallation javaInstallation, CancellationToken ct = default)
    {
        if (javaInstallation.JavaArtifactType != JavaArtifactType.Manifest) throw new InvalidOperationException("Manifest installer can only handle menifest installations.");
        if (Directory.Exists(javaInstallation.UninstallerPath)) Directory.Delete(javaInstallation.UninstallerPath, true);
        else throw new IOException($"Java installation {javaInstallation.UninstallerPath} does not exist.");
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

using System;
using Wave.Domain.Java;

namespace Wave.Infrastructure.Out.Java.JavaPackage;

public class ManifestJavaPackage : IJavaPackage
{
    public required string PackageDirectory { get; set; }
    public required string Filename { get; set; }

    public string PackagePath => Path.Combine(PackageDirectory, Filename);

    public required JavaSupplierType JavaSupplierType { get; set; }
    public required string JavaName { get; set; }
    public required int Version { get; set; }

    public void Dispose()
    {
        if (Directory.Exists(PackagePath))
            Directory.Delete(PackagePath, true);
    }
}

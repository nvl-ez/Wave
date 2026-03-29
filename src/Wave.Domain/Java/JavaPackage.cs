using System;

namespace Wave.Domain.Java;

public interface IJavaPackage : IDisposable
{
    public string PackageDirectory { get; set; }
    public string Filename { get; set; }
    public string PackagePath { get; }
    public JavaSupplierType JavaSupplierType { get; set; }
    public string JavaName { get; set; }
    public int Version { get; set; }
    public JavaArtifactType JavaArtifactType { get; set; }
}

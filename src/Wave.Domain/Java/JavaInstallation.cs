using System;

namespace Wave.Domain.Java;

public class JavaInstallation
{
    public required string ExecutableFile { get; set; }
    public required string UninstallerPath { get; set; }
    public required int Version { get; set; }
    public required string Name { get; set; }
    public required JavaSupplierType JavaSupplierType { get; set; }
    public required JavaArtifactType JavaArtifactType { get; set; }

    public bool Matches(JavaInstallation? other) =>
        other is not null &&
        Version == other.Version &&
        JavaSupplierType == other.JavaSupplierType &&
        JavaArtifactType == other.JavaArtifactType;
}

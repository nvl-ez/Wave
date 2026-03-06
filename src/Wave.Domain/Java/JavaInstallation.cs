using System;

namespace Wave.Domain.Java;

public record class JavaInstallation
{
    public required JavaArtifactType JavaArtifactType { get; set; }
    public required string ExecutablePath { get; set; }
    public required string UninstallerPath { get; set; }
    public required int Version { get; set; }
    public required string Name { get; set; }
    public required JavaSupplierType JavaSupplierType { get; set; }
}

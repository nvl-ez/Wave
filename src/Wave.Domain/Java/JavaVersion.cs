using System;
using Wave.Domain.Os;

namespace Wave.Domain.Java;

public record class JavaVersion
{
    public required int Version { get; set; }
    public required ArchitectureType ArchitectureType { get; set; }
    public required int ArchitectureBitType { get; set; }
    public required OsType OsType { get; set; }
    public required List<JavaArtifact> JavaArtifacts { get; set; }
}

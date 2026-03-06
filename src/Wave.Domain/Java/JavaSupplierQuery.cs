using System;
using Wave.Domain.Os;

namespace Wave.Domain.Java;

public record class JavaSupplierQuery
{
    public int? Version { get; set; } = null;
    public required OsType OsType { get; set; }
    public required ArchitectureType ArchitectureType { get; set; }
    public required int ArchitectureBitType { get; set; }
}

using System;
using Wave.Domain.Os;

namespace Wave.Domain.Java;

public record class JavaSupplierQuery
{
    public int? Version { get; set; } = null;
    public required OsType OsType { get; set; }
    public ArchitectureType ArchitectureType { get; set; }
    public int ArchitectureBitType { get; set; }
}

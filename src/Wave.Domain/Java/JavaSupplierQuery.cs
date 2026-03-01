using System;
using Wave.Domain.Os;

namespace Wave.Domain.Java;

public record class JavaSupplierQuery
{
    public int? Version { get; set; } = null;
    public OsType? OsType { get; set; } = null;
    public ArchitectureType? ArchitectureType { get; set; } = null;
    public int? ArchitectureBitType { get; set; } = null;
}

using System;
using Wave.Domain.System;

namespace Wave.Domain.Java;

public record class JavaSupplierQuery
{
    public required int Version { get; set; }
    public required OsType OsType { get; set; }
    public required ArchitectureType ArchitectureType { get; set; }
    public required int ArchitectureBitType { get; set; }
}

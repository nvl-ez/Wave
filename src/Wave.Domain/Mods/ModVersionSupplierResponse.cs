using System;
using Wave.Domain.Utils;

namespace Wave.Domain.Mods;

public record class ModVersionSupplierResponse
{
    public required IEnumerable<ModVersion> Versions { get; set; }
    public required PaginationState PaginationState { get; set; }
}

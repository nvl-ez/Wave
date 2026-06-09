using System;
using Wave.Domain.Utils;

namespace Wave.Domain.Mods;

public record class ModInfoSupplierResponse
{
    public required IEnumerable<ModInfo> Mods { get; set; }
    public required PaginationState PaginationState { get; set; }
}

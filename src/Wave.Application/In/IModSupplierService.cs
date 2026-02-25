using System;
using Wave.Domain.Mods;

namespace Wave.Application.In;

public interface IModSupplierService
{
    public Task<IEnumerable<ModInfo>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct);
    public Task<ModVersion> SearchModAsync(ModInfo mod, CancellationToken ct);
}

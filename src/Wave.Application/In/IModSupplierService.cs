using System;
using Wave.Domain.Mods;
using Wave.Domain.ModSupplier;

namespace Wave.Application.In;

public interface IModSupplierService
{
    public Task<IEnumerable<Mod>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct);
    public Task<Mod> SearchModAsync(Mod mod, CancellationToken ct);
}

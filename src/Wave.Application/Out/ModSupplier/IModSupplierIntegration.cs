using System;
using Wave.Domain.ModSupplier;
using Wave.Domain.Mods;

namespace Wave.Application.Out.ModSupplier;

public interface IModSupplierIntegration
{
    public Task<IEnumerable<Mod>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct);
    public Task<Mod> SearchModAsync(Mod mod, CancellationToken ct);

}

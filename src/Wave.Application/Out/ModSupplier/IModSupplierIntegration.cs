using System;
using Wave.Domain.Mods;

namespace Wave.Application.Out.ModSupplier;

public interface IModSupplierIntegration
{
    public Task<IEnumerable<ModInfo>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct);
    public Task<IEnumerable<ModVersion>> GetModVersionsAsync(ModInfo mod, CancellationToken ct);
}

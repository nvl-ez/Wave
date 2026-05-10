using System;
using Wave.Domain.Mods;

namespace Wave.Application.Out.ModSupplier;

public interface IModSupplierIntegration
{
    public Task<IEnumerable<ModInfo>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct = default);
    public Task<IEnumerable<ModVersion>> GetModVersionsAsync(ModInfo mod, CancellationToken ct = default);
    public Task DownloadMod(ModVersion modVersion, string modsPath, CancellationToken ct = default);
}

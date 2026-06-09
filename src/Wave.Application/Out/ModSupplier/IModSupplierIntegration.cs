using System;
using Wave.Domain.Mods;

namespace Wave.Application.Out.ModSupplier;

public interface IModSupplierIntegration
{
    public ModSupplierType ModSupplierType { get; }
    public bool CanHandle(ModSupplierType modSupplierType);
    public Task<ModInfoSupplierResponse> SearchModsAsync(ModInfoSupplierQuery modInfoSupplierQuery, CancellationToken ct = default);
    public Task<ModDetails> GetModDetailsAsync(string modId, CancellationToken ct = default);
    public Task<ModVersionSupplierResponse> GetModVersionsAsync(ModVersionSupplierQuery modVersionSupplierQuery, CancellationToken ct = default);
    public Task DownloadMod(ModVersion modVersion, string modsPath, CancellationToken ct = default);
}

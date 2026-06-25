using System;
using Wave.Domain.Mods;

namespace Wave.Application.Out.ModSupplier;

public interface IModSupplierIntegration
{
    public ModSupplierType ModSupplierType { get; }
    public bool CanHandle(ModSupplierType modSupplierType);
    public Task<ModInfoSupplierResponse> SearchModsAsync(ModInfoSupplierQuery modInfoSupplierQuery, CancellationToken ct = default);
    public Task<ModDetails> GetModDetailsAsync(ModBase modBase, CancellationToken ct = default);
    public Task<ModVersionSupplierResponse> GetModVersionsAsync(ModVersionSupplierQuery modVersionSupplierQuery, CancellationToken ct = default);
    public Task DownloadMod(ModFile modFile, string modsPath, CancellationToken ct = default);
}

using System;
using Wave.Domain.Mods;

namespace Wave.Application.In;

public interface IModCatalogService
{
    public Task<IEnumerable<KeyValuePair<ModSupplierType, string>>> GetModSupplierTypesAsync(CancellationToken ct = default);
    public Task<ModInfoSupplierResponse> SearchModsAsync(ModInfoSupplierQuery modInfoSupplierQuery, CancellationToken ct = default);
    public Task<ModDetails> GetModDetailsAsync(string modId, ModSupplierType modSupplierType, CancellationToken ct = default);
    public Task<ModVersionSupplierResponse> GetModVersionsAsync(ModVersionSupplierQuery modVersionSupplierQuery, CancellationToken ct = default);
}

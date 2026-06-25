using System;
using System.Globalization;
using Wave.Application.In;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;

namespace Wave.Infrastructure.In;

public class ModCatalogService : IModCatalogService
{
    private readonly IEnumerable<IModSupplierIntegration> modSuppliers;
    public ModCatalogService(IEnumerable<IModSupplierIntegration> modSuppliers)
    {
        this.modSuppliers = modSuppliers;
    }

    public async Task<IEnumerable<KeyValuePair<ModSupplierType, string>>> GetModSupplierTypesAsync(CancellationToken ct = default)
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        return modSuppliers.Select(
            m => new KeyValuePair<ModSupplierType, string>(m.ModSupplierType, ti.ToTitleCase(m.ModSupplierType.ToString()))
        );
    }

    public async Task<ModInfoSupplierResponse> SearchModsAsync(ModInfoSupplierQuery modInfoSupplierQuery, CancellationToken ct = default)
    {
        IModSupplierIntegration? target = null;
        foreach (var modSupplier in modSuppliers)
        {
            if (modSupplier.CanHandle(modInfoSupplierQuery.ModSupplierType))
            {
                target = modSupplier;
                break;
            }
        }

        if (target is null) throw new InvalidDataException($"There is no modsupplier that can handle the type {modInfoSupplierQuery.ModSupplierType}.");

        return await target.SearchModsAsync(modInfoSupplierQuery);
    }

    public async Task<ModDetails> GetModDetailsAsync(ModBase modBase, ModSupplierType modSupplierType, CancellationToken ct = default)
    {
        IModSupplierIntegration? target = null;
        foreach (var modSupplier in modSuppliers)
        {
            if (modSupplier.CanHandle(modSupplierType))
            {
                target = modSupplier;
                break;
            }
        }

        if (target is null) throw new InvalidDataException($"There is no modsupplier that can handle the type {modSupplierType}.");

        return await target.GetModDetailsAsync(modBase);
    }

    public async Task<ModVersionSupplierResponse> GetModVersionsAsync(ModVersionSupplierQuery modVersionSupplierQuery, CancellationToken ct = default)
    {
        IModSupplierIntegration? target = null;
        foreach (var modSupplier in modSuppliers)
        {
            if (modSupplier.CanHandle(modVersionSupplierQuery.ModSupplierType))
            {
                target = modSupplier;
                break;
            }
        }

        if (target is null) throw new InvalidDataException($"There is no modsupplier that can handle the type {modVersionSupplierQuery.ModSupplierType}.");

        return await target.GetModVersionsAsync(modVersionSupplierQuery);
    }


}

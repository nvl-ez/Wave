using System;
using System.Globalization;
using Wave.Application.In;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;

namespace Wave.Infrastructure.In;

public class ModCatalogService : IModCatalogService
{
    private readonly IEnumerable<IModSupplierIntegration> modSuppliers;
    private readonly IApplicationConfigurationService configurationService;
    public ModCatalogService(IEnumerable<IModSupplierIntegration> modSuppliers, IApplicationConfigurationService configurationService)
    {
        this.modSuppliers = modSuppliers;
        this.configurationService = configurationService;
    }

    public async Task<IEnumerable<KeyValuePair<ModSupplierType, string>>> GetModSupplierTypesAsync(CancellationToken ct = default)
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        await configurationService.GetAsync(ct);
        List<KeyValuePair<ModSupplierType, string>> modSupplierTypes = [];

        foreach (var modSupplier in modSuppliers)
        {
            if (modSupplier.RequiresToken && !modSupplier.HasToken)
                continue;

            modSupplierTypes.Add(new KeyValuePair<ModSupplierType, string>(
                modSupplier.ModSupplierType,
                ti.ToTitleCase(modSupplier.ModSupplierType.ToString())));
        }

        return modSupplierTypes;
    }

    public async Task<ModInfoSupplierResponse> SearchModsAsync(ModInfoSupplierQuery modInfoSupplierQuery, CancellationToken ct = default)
    {
        await configurationService.GetAsync(ct);
        IModSupplierIntegration? target = null;
        foreach (var modSupplier in modSuppliers)
        {
            if (modSupplier.RequiresToken && !modSupplier.HasToken)
                continue;

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
        await configurationService.GetAsync(ct);
        IModSupplierIntegration? target = null;
        foreach (var modSupplier in modSuppliers)
        {
            if (modSupplier.RequiresToken && !modSupplier.HasToken)
                continue;

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
        await configurationService.GetAsync(ct);
        IModSupplierIntegration? target = null;
        foreach (var modSupplier in modSuppliers)
        {
            if (modSupplier.RequiresToken && !modSupplier.HasToken)
                continue;

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

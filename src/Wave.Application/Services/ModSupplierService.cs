using System;
using Wave.Application.In;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;

namespace Wave.Application.Services;

public class ModSupplierService : IModSupplierService
{
    private readonly IModSupplierIntegration modSupplierIntegration;

    public ModSupplierService(IModSupplierIntegration modSupplierIntegration)
    {
        this.modSupplierIntegration = modSupplierIntegration;
    }

    public Task<ModVersion> SearchModAsync(ModInfo mod, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ModInfo>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct)
    {
        return await modSupplierIntegration.SearchModsAsync(modSupplierQuery, ct);
    }
}

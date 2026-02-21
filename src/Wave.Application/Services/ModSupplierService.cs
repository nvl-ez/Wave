using System;
using Wave.Application.In;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;
using Wave.Domain.ModSupplier;

namespace Wave.Application.Services;

public class ModSupplierService : IModSupplierService
{
    private readonly IModSupplierIntegration modSupplierIntegration;

    public ModSupplierService(IModSupplierIntegration modSupplierIntegration)
    {
        this.modSupplierIntegration = modSupplierIntegration;
    }

    public Task<Mod> SearchModAsync(Mod mod, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Mod>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct)
    {
        return await modSupplierIntegration.SearchModsAsync(modSupplierQuery, ct);
    }
}

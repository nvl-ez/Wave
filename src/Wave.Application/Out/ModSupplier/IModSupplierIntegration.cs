using System;
using Wave.Domain.ModSupplier;
using Wave.Domain.Mods;

namespace Wave.Application.Out.ModSupplier;

public interface IModSupplierIntegration
{
    public Task<IEnumerable<ModInfoResult>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct);
    public Task<IEnumerable<Mod>> GetModFilesAsync(ModInfoResult mod, CancellationToken ct);
}

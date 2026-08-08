using Wave.Application.In;
using Wave.Application.Out.Configuration;
using Wave.Domain.Configuration;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;

namespace Wave.Infrastructure.In;

public class ApplicationConfigurationService : IApplicationConfigurationService
{
    private readonly IApplicationConfigurationRepository configurationRepository;
    private readonly IEnumerable<IModSupplierIntegration> modSuppliers;

    public ApplicationConfigurationService(
        IApplicationConfigurationRepository configurationRepository,
        IEnumerable<IModSupplierIntegration> modSuppliers)
    {
        this.configurationRepository = configurationRepository;
        this.modSuppliers = modSuppliers;
    }

    public async Task<ApplicationConfiguration> GetAsync(CancellationToken ct = default)
    {
        var configuration = await configurationRepository.GetAsync(ct);
        ApplyToAdapters(configuration);
        return configuration;
    }

    public async Task SaveAsync(ApplicationConfiguration configuration, CancellationToken ct = default)
    {
        configuration.CurseforgeApiToken = string.IsNullOrWhiteSpace(configuration.CurseforgeApiToken)
            ? null
            : configuration.CurseforgeApiToken.Trim();

        ApplyToAdapters(configuration);
        await configurationRepository.SaveAsync(configuration, ct);
    }

    private void ApplyToAdapters(ApplicationConfiguration configuration)
    {
        foreach (var supplier in modSuppliers.Where(s => s.ModSupplierType == ModSupplierType.Curseforge))
            supplier.SetToken(configuration.CurseforgeApiToken);
    }
}

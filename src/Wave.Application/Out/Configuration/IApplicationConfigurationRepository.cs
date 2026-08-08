using Wave.Domain.Configuration;

namespace Wave.Application.Out.Configuration;

public interface IApplicationConfigurationRepository
{
    Task<ApplicationConfiguration> GetAsync(CancellationToken ct = default);
    Task SaveAsync(ApplicationConfiguration configuration, CancellationToken ct = default);
}

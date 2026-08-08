using Wave.Domain.Configuration;

namespace Wave.Application.In;

public interface IApplicationConfigurationService
{
    Task<ApplicationConfiguration> GetAsync(CancellationToken ct = default);
    Task SaveAsync(ApplicationConfiguration configuration, CancellationToken ct = default);
}

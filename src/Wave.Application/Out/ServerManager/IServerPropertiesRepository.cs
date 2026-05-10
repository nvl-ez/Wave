using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerPropertiesRepository
{
    public Task<Dictionary<string, string>> GetAllAsync(string propertiesPath, CancellationToken ct = default);
    public Task<string> GetAsync(string propertiesPath, string key, CancellationToken ct = default);

    public Task SetAsync(string propertiesPath, Dictionary<string, string> properties, CancellationToken ct = default);
}

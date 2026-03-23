using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerPropertiesRepository
{
    public Task<Dictionary<string, string>> GetAllAsync(Server server, CancellationToken ct = default);
    public Task<string> GetAsync(Server server, string key, CancellationToken ct = default);

    public Task SetAsync(Server server, CancellationToken ct = default);
    public Task SetAsync(Server server, string key, string value, CancellationToken ct = default);
}

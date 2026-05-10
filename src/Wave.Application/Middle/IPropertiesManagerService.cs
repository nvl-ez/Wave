using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Middle;

public interface IPropertiesManagerService
{
    public Task MergeSetPropertiesAsync(Server server, CancellationToken ct = default);
    public Task SetPropertiesAsync(Server server, CancellationToken ct = default);
    public Task<Dictionary<string, string>> TryGetPropertiesAsync(Server server, CancellationToken ct = default);
    public Task<string> TryGetPropertyAsync(Server server, string key, CancellationToken ct = default);
}

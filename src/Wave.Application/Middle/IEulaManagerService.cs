using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Middle;

public interface IEulaManagerService
{
    public Task SetEulaAsync(Server server, ServerQuery serverQuery, CancellationToken ct = default);
    public Task<bool> TryGetEulaAsync(Server server, CancellationToken ct = default);
}

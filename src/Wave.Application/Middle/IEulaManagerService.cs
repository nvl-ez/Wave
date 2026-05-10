using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Middle;

public interface IEulaManagerService
{
    public Task SetEulaAsync(Server server, CancellationToken ct = default);
    public Task<bool> TryGetEulaAsync(Server server, CancellationToken ct = default);
}

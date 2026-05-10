using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerEulaRepository
{
    public Task SetAsync(string eulaPath, bool value, CancellationToken ct = default);
    public Task<bool> GetAsync(string eulaPath, CancellationToken ct = default);
}

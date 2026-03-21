using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerHandlerService
{
    public Task CreateAsync(Server server, CancellationToken ct = default);
    public Task EditAsync(Server server, CancellationToken ct = default);
    public Task DeleteAsync(Server server, CancellationToken ct = default);
}

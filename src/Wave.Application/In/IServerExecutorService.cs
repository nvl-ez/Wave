using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerExecutorService
{
    public Task<ServerStartResult> Start(Guid id, CancellationToken ct = default);
    public IServerSession? TryGetSession(Guid id);
    public Task StopAll();
}

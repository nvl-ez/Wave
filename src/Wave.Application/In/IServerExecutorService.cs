using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerExecutorService
{
    public Task<IServerSession> Start(Guid id, CancellationToken ct = default);
    public Task<IServerSession> GetSession(Guid id);
}

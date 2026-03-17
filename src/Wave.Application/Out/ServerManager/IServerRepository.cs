using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerRepository
{
    public Task<IEnumerable<Server>> GetServersAsync(CancellationToken ct);
    public Task SaveAsync(Server server, CancellationToken ct);
    public Task DeleteAsync(Server server, CancellationToken ct);

    public IEnumerable<Server> GetServers();
    public void Save(Server server);
    public void Delete(Server serves);
}

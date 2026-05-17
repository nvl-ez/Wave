using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerRepository
{
    public Task<IEnumerable<Server>> GetAllServersAsync(CancellationToken ct = default);
    public Task SaveServerAsync(Server server, CancellationToken ct = default);
    public Task DeleteServerAsync(Guid id, CancellationToken ct = default);

    public IEnumerable<Server> GetAllServers();
    public void SaveServer(Server server);
    public void DeleteServer(Guid id);
}

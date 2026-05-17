using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerManagerService
{
    public Task CreateServerAsync(ServerCreationQuery serverCreationQuery, CancellationToken ct = default);
    public Task EditServerAsync(Server server, CancellationToken ct = default);
    public Task DeleteServerAsync(Server server, CancellationToken ct = default);

    public Task<Server> GetServerAsync(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<Server>> GetAllServersAsync(CancellationToken ct = default);
    public Server GetServer(Guid id);
    public IEnumerable<Server> GetAllServers();
}

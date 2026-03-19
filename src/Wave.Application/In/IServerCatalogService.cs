using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerCatalogService
{
    public Task<IEnumerable<Server>> GetServersAsync(CancellationToken ct = default);
    public Task<Server> GetServerAsync(Guid id, CancellationToken ct = default);
    public Task SaveAsync(Server server, CancellationToken ct = default);
    public Task DeleteAsync(Server server, CancellationToken ct = default);
    public Task DeleteAsync(Guid id, CancellationToken ct = default);

    public IEnumerable<Server> GetServers();
    public Server GetServer(Guid id);
    public void Save(Server server);
    public void Delete(Server server);
    public void Delete(Guid id);
}

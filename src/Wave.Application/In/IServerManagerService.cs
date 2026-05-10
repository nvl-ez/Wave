using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerManagerService
{
    public Task CreateAsync(Server server, CancellationToken ct = default);
    public Task EditAsync(Server server, CancellationToken ct = default);
    public Task DeleteAsync(Server server, CancellationToken ct = default);
    public Task<Server> LoadServerAsync(Guid id, CancellationToken ct = default);

    public Task<ServerInfo> GetServerInfoAsync(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<ServerInfo>> GetAllAsync(CancellationToken ct = default);
    public ServerInfo GetServerInfo(Guid id);
    public IEnumerable<ServerInfo> GetAll();
}

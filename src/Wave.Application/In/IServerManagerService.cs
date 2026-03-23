using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerManagerService
{
    public Task CreateAsync(Server server, CancellationToken ct = default);
    public Task EditAsync(Server server, CancellationToken ct = default);
    public Task DeleteAsync(Server server, CancellationToken ct = default);
    public Task<Server> LoadAsync(ServerInfo serverInfo, CancellationToken ct = default);
    public Task<Server> LoadAsync(Guid id, CancellationToken ct = default);

    public Task<ServerInfo> GetAsync(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<ServerInfo>> GetAllAsync(CancellationToken ct = default);
    public ServerInfo Get(Guid id);
    public IEnumerable<ServerInfo> GetAll();
}

using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerManagerService
{
    public Task CreateServerAsync(Server server, CancellationToken ct = default);
    public Task EditServerAsync(Server server, CancellationToken ct = default);
    public Task DeleteServerAsync(Server server, CancellationToken ct = default);
    public Task<Server> LoadServerAsync(Guid id, CancellationToken ct = default);

    public Task<ServerInfo> GetServerInfoAsync(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<ServerInfo>> GetAllServerInfosAsync(CancellationToken ct = default);
    public ServerInfo GetServerInfo(Guid id);
    public IEnumerable<ServerInfo> GetAllServerInfos();
}

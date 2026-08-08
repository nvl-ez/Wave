using System;
using Wave.Domain.ServerManager;
using Wave.Domain.Java;

namespace Wave.Application.In;

public interface IServerManagerService
{
    public Task<ServerQuery> CreateServerAsync(ServerQuery serverCreationQuery, CancellationToken ct = default);
    public Task<ServerChanges?> EditServerAsync(ServerQuery server, CancellationToken ct = default);
    public Task DeleteServerAsync(Guid id, CancellationToken ct = default);
    public Task SetServerIconAsync(Guid id, Stream image, CancellationToken ct = default);
    public string? GetServerIconPath(Guid id);

    public Task<ServerQuery> GetServerQueryAsync(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<ServerQuery>> GetAllServerQueriesAsync(CancellationToken ct = default);

    public Task<Server> GetServerAsync(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<Server>> GetAllServersAsync(CancellationToken ct = default);
    public Task SetJavaInstallationForAllAsync(JavaInstallation? installation, CancellationToken ct = default);
    public Server GetServer(Guid id);
    public IEnumerable<Server> GetAllServers();
}

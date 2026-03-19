using System;
using Wave.Application.In;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.In;

public class ServerCatalogService : IServerCatalogService
{
    private readonly IServerRepository serverRepository;
    public ServerCatalogService(IServerRepository serverRepository)
    {
        this.serverRepository = serverRepository;
    }
    public void Delete(Server server)
    {
        Guid id = server.Id;
        serverRepository.Delete(id);
    }

    public void Delete(Guid id)
    {
        serverRepository.Delete(id);
    }

    public async Task DeleteAsync(Server server, CancellationToken ct = default)
    {
        Guid id = server.Id;
        await serverRepository.DeleteAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await serverRepository.DeleteAsync(id, ct);
    }

    public Server GetServer(Guid id)
    {
        return serverRepository.GetServers().First(s => s.Id == id);
    }

    public async Task<Server> GetServerAsync(Guid id, CancellationToken ct = default)
    {
        return (await serverRepository.GetServersAsync(ct)).First(s => s.Id == id);
    }

    public IEnumerable<Server> GetServers()
    {
        return serverRepository.GetServers();
    }

    public async Task<IEnumerable<Server>> GetServersAsync(CancellationToken ct = default)
    {
        return await serverRepository.GetServersAsync(ct);
    }

    public void Save(Server server)
    {
        serverRepository.Save(server);
    }

    public async Task SaveAsync(Server server, CancellationToken ct = default)
    {
        await serverRepository.SaveAsync(server, ct);
    }
}

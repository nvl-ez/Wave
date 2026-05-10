using System;
using Wave.Application.In;
using Wave.Application.Out.Java;
using Wave.Application.Out.ServerManager;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;
using Wave.Infrastructure.Exceptions;

namespace Wave.Infrastructure.In;

public class ServerExecutorService : IServerExecutorService
{
    private readonly IServerExecutor serverExecutor;
    private readonly IJavaInstallRepository javaInstallRepository;
    private readonly IServerRepository serverRepository;

    private Dictionary<Guid, IServerSession> runningServers = new(); //TODO abstract dictionary in an out port

    public ServerExecutorService(IServerExecutor serverExecutor, IServerRepository serverRepository, IJavaInstallRepository javaInstallRepository)
    {
        this.serverExecutor = serverExecutor;
        this.serverRepository = serverRepository;
        this.javaInstallRepository = javaInstallRepository;
    }

    public IServerSession? TryGetSession(Guid id)
    {
        if (!runningServers.ContainsKey(id)) return null;
        return runningServers[id];
    }

    public async Task<IServerSession> Start(Guid id, CancellationToken ct = default)
    {
        Server server = (await serverRepository.GetAllAsync()).First(s => s.Id == id);

        int? serverJavaVersion = server.Details.MinecraftVersionDetails?.JavaVersion;

        if (serverJavaVersion is null) throw new JavaInstallationNotFoundException($"Server does not have a required Java version.");

        JavaInstallation? javaInstallation = (await javaInstallRepository.GetAllAsync())
            .Where(j => j.Version >= serverJavaVersion)
            .OrderBy(j => j.Version)
            .FirstOrDefault();

        if (javaInstallation is null) throw new JavaInstallationNotFoundException($"No available Java installation was found for version {serverJavaVersion}.");

        if (runningServers.ContainsKey(server.Id)) throw new ServerAlreadyRunningException($"Server '{server.Info.Name}' is already running.");

        IServerSession serverSession = serverExecutor.Start(server, javaInstallation);

        runningServers.Add(server.Id, serverSession);
        serverSession.ServerDisposed += ServerDisposed;

        return serverSession;
    }

    private void ServerDisposed(object? sender, Guid id)
    {
        if (!runningServers.ContainsKey(id)) throw new ServerNotRunningException("Attempted to find a server that is not running.");

        IServerSession serverSession = runningServers[id];
        serverSession.ServerDisposed -= ServerDisposed;
        runningServers.Remove(id);
    }

    //TODO: Kill all servers running on exit
}

using System;
using Wave.Application.In;
using Wave.Application.Middle;
using Wave.Application.Out.Java;
using Wave.Application.Out.ServerManager;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;
using Wave.Infrastructure.Exceptions;

namespace Wave.Infrastructure.In;

public class ServerExecutorService : IServerExecutorService
{
    private readonly IServerPathResolver serverPathResolver;
    private readonly IServerExecutor serverExecutor;
    private readonly IJavaInstallRepository javaInstallRepository;
    private readonly IServerRepository serverRepository;
    private readonly IApplicationConfigurationService configurationService;

    private readonly Dictionary<Guid, IServerSession> runningServers = new(); //TODO abstract dictionary in an out port
    private readonly Lock runningServersLock = new();

    public ServerExecutorService(IServerPathResolver serverPathResolver, IServerExecutor serverExecutor, IServerRepository serverRepository, IJavaInstallRepository javaInstallRepository, IApplicationConfigurationService configurationService)
    {
        this.serverPathResolver = serverPathResolver;
        this.serverExecutor = serverExecutor;
        this.serverRepository = serverRepository;
        this.javaInstallRepository = javaInstallRepository;
        this.configurationService = configurationService;
    }

    public IServerSession? TryGetSession(Guid id)
    {
        lock (runningServersLock)
        {
            if (!runningServers.ContainsKey(id)) return null;
            return runningServers[id];
        }
    }

    public async Task<ServerStartResult> Start(Guid id, CancellationToken ct = default)
    {
        Server server = (await serverRepository.GetAllServersAsync()).First(s => s.Id == id);

        int? serverJavaVersion = server.MinecraftVersionInstallation?.JavaVersion;

        if (serverJavaVersion is null) throw new JavaInstallationNotFoundException($"Server does not have a required Java version. Has a jar been downloaded?");

        IEnumerable<JavaInstallation> installations = await javaInstallRepository.GetAllAsync(ct);
        JavaInstallation? configuredInstallation = server.JavaInstallation
            ?? (await configurationService.GetAsync(ct)).JavaInstallation;
        JavaInstallation? javaInstallation = configuredInstallation is not null
            ? installations.FirstOrDefault(j => j.Matches(configuredInstallation))
            : installations.FirstOrDefault(j => j.Version == serverJavaVersion)
                ?? installations.Where(j => j.Version > serverJavaVersion).OrderBy(j => j.Version).FirstOrDefault();

        if (javaInstallation is null)
        {
            return ServerStartResult.JavaNotFound(serverJavaVersion.Value);
        }

        lock (runningServersLock)
        {
            if (runningServers.ContainsKey(server.Id)) throw new ServerAlreadyRunningException($"Server '{server.Name}' is already running.");

            IServerSession serverSession = serverExecutor.Start(
                server.Id,
                serverPathResolver.GetServerRootDirectory(server),
                serverPathResolver.GetServerJarPath(server),
                javaInstallation,
                server.ExecutionFlags,
                ct
                );

            runningServers.Add(server.Id, serverSession);
            serverSession.ServerDisposed += ServerDisposed;

            return ServerStartResult.Success(serverSession);
        }
    }

    public async Task StopAll()
    {
        IServerSession[] serverSessions;

        lock (runningServersLock)
        {
            serverSessions = runningServers.Values.ToArray();
        }

        await Task.WhenAll(serverSessions.Select(session => session.DisposeAsync().AsTask()));
    }

    private void ServerDisposed(object? sender, Guid id)
    {
        lock (runningServersLock)
        {
            if (!runningServers.Remove(id, out IServerSession? serverSession)) return;
            serverSession.ServerDisposed -= ServerDisposed;
        }
    }
}

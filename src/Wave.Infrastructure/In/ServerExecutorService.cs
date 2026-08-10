using System;
using Wave.Application.In;
using Wave.Application.Middle;
using Wave.Application.Out.Java;
using Wave.Application.Out.ServerManager;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;
using Wave.Domain.System;
using Wave.Infrastructure.Exceptions;

namespace Wave.Infrastructure.In;

public class ServerExecutorService : IServerExecutorService
{
    private readonly IServerPathResolver serverPathResolver;
    private readonly IServerExecutor serverExecutor;
    private readonly IJavaInstallRepository javaInstallRepository;
    private readonly IServerRepository serverRepository;
    private readonly IApplicationConfigurationService configurationService;
    private readonly IPortMapper portMapper;

    private readonly Dictionary<Guid, RunningServer> runningServers = new(); //TODO abstract dictionary in an out port
    private readonly HashSet<int> reservedPorts = [];
    private readonly Lock runningServersLock = new();

    public ServerExecutorService(IServerPathResolver serverPathResolver, IServerExecutor serverExecutor, IServerRepository serverRepository, IJavaInstallRepository javaInstallRepository, IApplicationConfigurationService configurationService, IPortMapper portMapper)
    {
        this.serverPathResolver = serverPathResolver;
        this.serverExecutor = serverExecutor;
        this.serverRepository = serverRepository;
        this.javaInstallRepository = javaInstallRepository;
        this.configurationService = configurationService;
        this.portMapper = portMapper;
    }

    public IServerSession? TryGetSession(Guid id)
    {
        lock (runningServersLock)
        {
            if (!runningServers.ContainsKey(id)) return null;
            return runningServers[id].Session;
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

        int port = GetServerPort(server);
        lock (runningServersLock)
        {
            if (runningServers.ContainsKey(server.Id)) throw new ServerAlreadyRunningException($"Server '{server.Name}' is already running.");
            if (!reservedPorts.Add(port)) return ServerStartResult.PortInUse(port);
        }

        PortMappingLease portMapping;
        try
        {
            portMapping = await portMapper.OpenAsync(port, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            ReleasePort(port);
            throw;
        }
        catch
        {
            ReleasePort(port);
            return ServerStartResult.PortMappingFailed(port);
        }

        try
        {
            IServerSession serverSession;
            lock (runningServersLock)
            {
                serverSession = serverExecutor.Start(
                server.Id,
                serverPathResolver.GetServerRootDirectory(server),
                serverPathResolver.GetServerJarPath(server),
                javaInstallation,
                server.ExecutionFlags,
                ct
                );

                runningServers.Add(server.Id, new RunningServer(serverSession, port, portMapping));
                serverSession.ServerDisposed += ServerDisposed;
            }

            return ServerStartResult.Success(serverSession, port);
        }
        catch
        {
            await portMapping.DisposeAsync();
            ReleasePort(port);
            throw;
        }
    }

    public async Task StopAll()
    {
        IServerSession[] serverSessions;

        lock (runningServersLock)
        {
            serverSessions = runningServers.Values.Select(server => server.Session).ToArray();
        }

        await Task.WhenAll(serverSessions.Select(session => session.DisposeAsync().AsTask()));
    }

    private async void ServerDisposed(object? sender, Guid id)
    {
        RunningServer? runningServer;
        lock (runningServersLock)
        {
            if (!runningServers.Remove(id, out runningServer)) return;
            runningServer.Session.ServerDisposed -= ServerDisposed;
            reservedPorts.Remove(runningServer.Port);
        }

        try
        {
            await runningServer.PortMapping.DisposeAsync();
        }
        catch
        {
            // The server is already stopped. A router failure while removing the mapping
            // must not leave the local port marked as occupied.
        }
    }

    private void ReleasePort(int port)
    {
        lock (runningServersLock)
        {
            reservedPorts.Remove(port);
        }
    }

    private static int GetServerPort(Server server)
    {
        const int defaultMinecraftPort = 25565;
        return server.Properties.TryGetValue("server-port", out string? value)
            && int.TryParse(value, out int port)
            && port is >= 1 and <= 65535
                ? port
                : defaultMinecraftPort;
    }

    private sealed record RunningServer(IServerSession Session, int Port, PortMappingLease PortMapping);
}

using System;
using System.Text;
using Wave.Application.In;
using Wave.Application.Middle;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;
using Wave.Infrastructure.Middle;

namespace Wave.Infrastructure.In;

public class ServerManagerService : IServerManagerService
{
    private readonly IServerPathResolver serverPathResolver;
    private readonly IServerRepository serverRepository;
    private readonly IVersionManagerService versionManagerService;
    private readonly IPropertiesManagerService propertiesManagerService;
    private readonly IEulaManagerService eulaManagerService;
    private readonly IModloaderManagerService modloaderManagerService;

    public ServerManagerService(
        IServerPathResolver serverPathResolver,
        IServerRepository serverRepository,
        IVersionManagerService versionManagerService,
        IPropertiesManagerService propertiesManagerService,
        IEulaManagerService eulaManagerService,
        IModloaderManagerService modloaderManagerService)
    {
        this.serverRepository = serverRepository;
        this.serverPathResolver = serverPathResolver;
        this.versionManagerService = versionManagerService;
        this.propertiesManagerService = propertiesManagerService;
        this.eulaManagerService = eulaManagerService;
        this.modloaderManagerService = modloaderManagerService;
    }

    public async Task<ServerQuery> CreateServerAsync(ServerQuery query, CancellationToken ct = default)
    {
        Server server = new()
        {
            Name = query.Name,
            Properties = query.Properties,
            Eula = query.Eula,
        };

        serverPathResolver.CreateServerRootDirectory(server);

        //Download server files
        server = await versionManagerService.SetVersionAsync(server, query);

        //Create server.properties
        serverPathResolver.CreateServerPropertiesFile(server);
        await propertiesManagerService.SetPropertiesAsync(server);

        //Create eula
        serverPathResolver.CreateEulaFile(server);
        await eulaManagerService.SetEulaAsync(server, query);

        //Add modloader if necessary
        ModloaderInfo? modloaderInfo = query.Modloader;
        if (modloaderInfo != null) await modloaderManagerService.AddModloaderAsync(server, modloaderInfo);

        // Save Server
        await serverRepository.SaveServerAsync(server);

        return new ServerQuery(server);
    }

    public async Task DeleteServerAsync(Guid id, CancellationToken ct = default)
    {
        Server server = await GetServerAsync(id);
        string serverPath = serverPathResolver.GetServerRootDirectory(server);

        if (!Directory.Exists(serverPath)) throw new IOException($"Directory '{serverPath}' does not exist.");

        Directory.Delete(serverPath, true);

        await serverRepository.DeleteServerAsync(server.Id);
    }

    public async Task EditServerAsync(ServerQuery query, CancellationToken ct = default)
    {
        /************
        * DIFFERING *
        ************/
        Server server = await GetServerAsync((Guid)query.Id);

        //Version
        await versionManagerService.SetVersionAsync(server, query);

        //Save Properties
        await propertiesManagerService.MergeSetPropertiesAsync(server, query);

        // Save Eula
        await eulaManagerService.SetEulaAsync(server, query);

        // Save server
        await serverRepository.SaveServerAsync(server);
    }

    public Server GetServer(Guid id)
    {
        return serverRepository.GetAllServers().First(s => s.Id == id); //TODO: Cargar informacion del servidor desde los archivos like Async
    }

    //TODO: CUIDADO: Get all servers devuelve datos de los servidores que pueden estar outdated si se han modificado los archivos manualmente.
    public IEnumerable<Server> GetAllServers()
    {
        return serverRepository.GetAllServers().ToList();
    }

    public async Task<IEnumerable<Server>> GetAllServersAsync(CancellationToken ct = default)
    {
        return (await serverRepository.GetAllServersAsync()).ToList();
    }

    public async Task<Server> GetServerAsync(Guid id, CancellationToken ct = default)
    {
        Server server = (await serverRepository.GetAllServersAsync()).First(s => s.Id == id);

        //Load Properties
        server.Properties = await propertiesManagerService.TryGetPropertiesAsync(server);

        // Load Eula
        server.Eula = await eulaManagerService.TryGetEulaAsync(server);

        return server;
    }

    public async Task<ServerQuery> GetServerQueryAsync(Guid id, CancellationToken ct = default)
    {
        return new ServerQuery(await GetServerAsync(id));
    }

    public async Task<IEnumerable<ServerQuery>> GetAllServerQueriesAsync(CancellationToken ct = default)
    {
        return (await GetAllServersAsync()).Select(s => new ServerQuery(s));
    }
}

using System;
using System.Text;
using Wave.Application.In;
using Wave.Application.Middle;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain.Mods;
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
    private readonly IModManagerService modManagerService;
    private readonly IImageTransformer imageTransformer;

    public ServerManagerService(
        IServerPathResolver serverPathResolver,
        IServerRepository serverRepository,
        IVersionManagerService versionManagerService,
        IPropertiesManagerService propertiesManagerService,
        IEulaManagerService eulaManagerService,
        IModloaderManagerService modloaderManagerService,
        IModManagerService modManagerService,
        IImageTransformer imageTransformer)
    {
        this.serverRepository = serverRepository;
        this.serverPathResolver = serverPathResolver;
        this.versionManagerService = versionManagerService;
        this.propertiesManagerService = propertiesManagerService;
        this.eulaManagerService = eulaManagerService;
        this.modloaderManagerService = modloaderManagerService;
        this.modManagerService = modManagerService;
        this.imageTransformer = imageTransformer;
    }

    public async Task<ServerQuery> CreateServerAsync(ServerQuery query, CancellationToken ct = default)
    {
        Server server = new()
        {
            Name = query.Name,
            ExecutionFlags = query.ExecutionFlags,
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

        //Add modloader
        await modloaderManagerService.AddModloaderAsync(server, query);

        //Add server
        await modManagerService.SetModsAsync(server, query);

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

    public async Task SetServerIconAsync(Guid id, Stream image, CancellationToken ct = default)
    {
        Server server = await GetServerAsync(id, ct);
        string iconPath = serverPathResolver.GetServerIconPath(server);
        string temporaryIconPath = $"{iconPath}.tmp";

        try
        {
            await using (FileStream output = File.Create(temporaryIconPath))
                await imageTransformer.TransformToPngAsync(image, output, 64, 64, ct);

            File.Move(temporaryIconPath, iconPath, true);
            server.ImageFilename = server.ServerPaths.ImageFilename;
            await serverRepository.SaveServerAsync(server, ct);
        }
        finally
        {
            if (File.Exists(temporaryIconPath))
                File.Delete(temporaryIconPath);
        }
    }

    public string? GetServerIconPath(Guid id)
    {
        Server server = GetServer(id);
        string iconPath = serverPathResolver.GetServerIconPath(server);
        return File.Exists(iconPath) ? iconPath : null;
    }

    public async Task<ServerChanges?> EditServerAsync(ServerQuery query, CancellationToken ct = default)
    {
        ServerChanges changes = new();
        /************
        * DIFFERING *
        ************/
        Server server = await GetServerAsync((Guid)query.Id);
        string originalVersion = server.MinecraftVersionInstallation!.MinecraftVersion;
        ModloaderBase? originalModloader = server.Modloader;
        server.ExecutionFlags = query.ExecutionFlags;

        //Version
        await versionManagerService.SetVersionAsync(server, query);

        //Save Properties
        await propertiesManagerService.MergeSetPropertiesAsync(server, query);

        // Save Eula
        await eulaManagerService.SetEulaAsync(server, query);

        //Manage Modloader
        if (server.Modloader?.Equals(query.Modloader) != true)
        {
            if (server.Modloader is not null)
                await modloaderManagerService.RemoveModloaderAsync(server);
            if (query.Modloader is not null)
                await modloaderManagerService.AddModloaderAsync(server, query);
        }

        if (
            server.Modloader is not null &&
            server.Modloader.ModloaderType == query.Modloader?.ModloaderType &&
            (server.Modloader.MinecraftVersion != originalVersion || server.Modloader.MinecraftVersion != query.MinecraftVersionBase!.MinecraftVersion)
        )
        {
            if (!await modloaderManagerService.MigrateModloaderAsync(server))
            {
                changes.DeletedModloader = originalModloader;
            }
        }

        //Eliminar mods si no hay modloader.
        if (query.Modloader is null && query.Mods.Count() > 0)
        {
            changes.DeletedMods = query.Mods;
            query.Mods = [];
        }

        //Manage Mods
        if (query.Modloader is not null && (originalVersion != query.MinecraftVersionBase?.MinecraftVersion || originalModloader?.ModloaderType != query.Modloader.ModloaderType))
        { //Update mods
            var failedMods = await modManagerService.SetModsAsync(server, query);
            var migrationResult = await modManagerService.MigrateModsAsync(server);
            changes.DeletedMods = migrationResult.DeletedMods;
            changes.FailedMods = failedMods.Concat(migrationResult.FailedMods);
        }
        else
        {
            if (query.Mods.Count() == 0)
                server.Mods = [];
            else
                changes.FailedMods = await modManagerService.SetModsAsync(server, query);
        }

        // Save server
        await serverRepository.SaveServerAsync(server);

        return (changes.DeletedMods?.Any() == true) ||
               (changes.FailedMods?.Any() == true) ||
               changes.DeletedModloader is not null
            ? changes
            : null;
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

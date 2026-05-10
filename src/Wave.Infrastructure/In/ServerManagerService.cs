using System;
using System.Text;
using Wave.Application.In;
using Wave.Application.Middle;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;
using Wave.Infrastructure.Middle;

namespace Wave.Infrastructure.In;

public class ServerManagerService : IServerManagerService
{
    private readonly string serversDirectory;
    private readonly IServerRepository serverRepository;
    private readonly IVersionManagerService versionManagerService;
    private readonly IPropertiesManagerService propertiesManagerService;
    private readonly IEulaManagerService eulaManagerService;

    public ServerManagerService(
        string serversDirectory,
        IServerRepository serverRepository,
        IVersionManagerService versionManagerService,
        IPropertiesManagerService propertiesManagerService,
        IEulaManagerService eulaManagerService)
    {
        this.serverRepository = serverRepository;
        this.serversDirectory = serversDirectory;
        this.versionManagerService = versionManagerService;
        this.propertiesManagerService = propertiesManagerService;
        this.eulaManagerService = eulaManagerService;
    }

    public async Task CreateAsync(Server server, CancellationToken ct = default)
    {
        string serverDirectory = Path.Combine(serversDirectory, server.Info.Name);
        Directory.CreateDirectory(serverDirectory);
        server.Info.ServerDirectory = serverDirectory;

        if (server.Details.MinecraftVersion is null) throw new NullReferenceException("The Minecraft version was not specified.");

        //Download files
        server = await versionManagerService.SetVersionAsync(server);


        //Create server.properties
        File.Create(server.PropertiesPath!).Close();
        await propertiesManagerService.SetPropertiesAsync(server);


        //Create eula
        await eulaManagerService.SetEulaAsync(server);

        // Save Server
        await serverRepository.SaveAsync(server);
    }

    public async Task DeleteAsync(Server server, CancellationToken ct = default)
    {
        if (server.Info.ServerDirectory is null) throw new NullReferenceException("Server directory cannot be null.");
        if (!Directory.Exists(server.Info.ServerDirectory)) throw new IOException($"Directory '{server.Info.ServerDirectory}' does not exist.");

        Directory.Delete(server.Info.ServerDirectory, true);

        await serverRepository.DeleteAsync(server.Id);
    }

    public async Task EditAsync(Server server, CancellationToken ct = default)
    {
        if (server.Info.ServerDirectory is null) throw new NullReferenceException("Server directory cannot be null.");
        if (!Directory.Exists(server.Info.ServerDirectory)) throw new IOException($"Directory '{server.Info.ServerDirectory}' does not exist.");

        /************
        * DIFFERING *
        ************/
        Server old = await LoadServerAsync(server.Id);

        //Version
        if (!string.Equals(old.Details.MinecraftVersion?.Version, server.Details.MinecraftVersion?.Version))
        {
            await versionManagerService.SetVersionAsync(server);
        }

        //Save Properties
        await propertiesManagerService.MergeSetPropertiesAsync(server);

        // Save Eula
        await eulaManagerService.SetEulaAsync(server);

        // Save server
        await serverRepository.SaveAsync(server);
    }

    public ServerInfo GetServerInfo(Guid id)
    {
        return serverRepository.GetAll().First(s => s.Info.Id == id).Info;
    }

    public IEnumerable<ServerInfo> GetAll()
    {
        return serverRepository.GetAll().Select(s => s.Info).ToList();
    }

    public async Task<IEnumerable<ServerInfo>> GetAllAsync(CancellationToken ct = default)
    {
        return (await serverRepository.GetAllAsync()).Select(s => s.Info).ToList();
    }

    public async Task<ServerInfo> GetServerInfoAsync(Guid id, CancellationToken ct = default)
    {
        return (await serverRepository.GetAllAsync()).First(s => s.Info.Id == id).Info;
    }

    public async Task<Server> LoadServerAsync(Guid id, CancellationToken ct = default)
    {
        Server server = (await serverRepository.GetAllAsync()).First(s => s.Id == id);
        ServerDetails details = server.Details;

        //Load Properties
        server.Details.Properties = await propertiesManagerService.TryGetPropertiesAsync(server);


        // Load Eula
        server.Details.Eula = await eulaManagerService.TryGetEulaAsync(server);

        return server;
    }
}

using System;
using System.Text;
using Wave.Application.In;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.In;

public class ServerManagerService : IServerManagerService
{
    private const string propertiesFilename = "server.properties";
    private readonly string serversDirectory;
    private readonly IServerRepository serverRepository;
    private readonly IMinecraftVersionRepository minecraftVersionRepository;
    private readonly IServerPropertiesRepository serverPropertiesRepository;
    public ServerManagerService(string serversDirectory, IServerRepository serverRepository, IMinecraftVersionRepository minecraftVersionRepository, IServerPropertiesRepository serverPropertiesRepository)
    {
        this.serverRepository = serverRepository;
        this.serversDirectory = serversDirectory;
        this.minecraftVersionRepository = minecraftVersionRepository;
        this.serverPropertiesRepository = serverPropertiesRepository;
    }

    public async Task CreateAsync(Server server, CancellationToken ct = default)
    {
        string serverDirectory = Path.Combine(serversDirectory, server.Info.Name);
        Directory.CreateDirectory(serverDirectory);
        server.Info.ServerDirectory = serverDirectory;

        if (server.Details.MinecraftVersion is null) throw new NullReferenceException("The Minecraft version was not specified.");

        //Download files
        string serverFilename = "server.jar";
        server.Details.MinecraftVersion = await minecraftVersionRepository.GetDetailsAsync(server.Details.MinecraftVersion);

        server.Details.MinecraftVersion = await minecraftVersionRepository.Download(server.Details.MinecraftVersion, serverFilename, serverDirectory);

        server.Details.ServerFilename = serverFilename;

        //Create server.properties
        string propertiesFile = Path.Combine(serverDirectory, propertiesFilename);
        File.Create(propertiesFile).Close();
        server.Details.PropertiesFilename = propertiesFilename;

        await serverPropertiesRepository.SetAsync(server);

        serverRepository.Save(server);
    }

    public async Task DeleteAsync(Server server, CancellationToken ct = default)
    {
        if (server.Info.ServerDirectory is null) throw new NullReferenceException("Server directory cannot be null.");
        if (!Directory.Exists(server.Info.ServerDirectory)) throw new IOException($"Directory '{server.Info.ServerDirectory}' does not exist.");

        Directory.Delete(server.Info.ServerDirectory, true);

        await serverRepository.DeleteAsync(server.Id);
    }

    public Task EditAsync(Server server, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public ServerInfo Get(Guid id)
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

    public async Task<ServerInfo> GetAsync(Guid id, CancellationToken ct = default)
    {
        return (await serverRepository.GetAllAsync()).First(s => s.Info.Id == id).Info;
    }

    public async Task<Server> LoadAsync(ServerInfo serverInfo, CancellationToken ct = default)
    {
        Server server = (await serverRepository.GetAllAsync()).First(s => s.Id == serverInfo.Id);

        var properties = await serverPropertiesRepository.GetAllAsync(server);

        server.Details.Properties = properties;

        return server;
    }

    public async Task<Server> LoadAsync(Guid id, CancellationToken ct = default)
    {
        Server server = (await serverRepository.GetAllAsync()).First(s => s.Id == id);
        ServerDetails details = server.Details;

        var properties = await serverPropertiesRepository.GetAllAsync(server);

        if (properties.Count > 0)
        {
            foreach (var property in properties)
            {
                if (details.Properties.ContainsKey(property.Key))
                {
                    details.Properties[property.Key] = property.Value;
                }
                else
                {
                    details.Properties.Add(property.Key, property.Value);
                }
            }

            server.Details.Properties = properties;
        }

        return server;
    }
}

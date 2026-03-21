using System;
using System.Text;
using Wave.Application.In;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.In;

public class ServerHandlerService : IServerHandlerService
{
    private readonly string serversDirectory;
    private readonly IServerRepository serverRepository;
    private readonly IMinecraftVersionRepository minecraftVersionRepository;
    public ServerHandlerService(string serversDirectory, IServerRepository serverRepository, IMinecraftVersionRepository minecraftVersionRepository)
    {
        this.serverRepository = serverRepository;
        this.serversDirectory = serversDirectory;
        this.minecraftVersionRepository = minecraftVersionRepository;
    }

    public async Task CreateAsync(Server server, CancellationToken ct = default)
    {
        string serverDirectory = Path.Combine(serversDirectory, server.Name);
        Directory.CreateDirectory(serverDirectory);
        server.ServerDirectory = serverDirectory;

        if (server.MinecraftVersion is null) throw new NullReferenceException("The Minecraft version was not specified.");

        //Download files
        string serverFilename = "server.jar";
        server.MinecraftVersion = await minecraftVersionRepository.GetDetailsAsync(server.MinecraftVersion);

        server.MinecraftVersion = await minecraftVersionRepository.Download(server.MinecraftVersion, serverFilename, serverDirectory);

        server.ServerFilename = serverFilename;

        //Create server.properties
        string propertiesFile = Path.Combine(serverDirectory, "server.properties");
        File.Create(propertiesFile);
        server.ServerPropertiesFilename = "server.properties";

        StringBuilder propertiesStringBuilder = new StringBuilder();

        foreach (var property in server.Properties)
        {
            propertiesStringBuilder.Append($"{property.Key}={property.Value}\n");
        }

        using (StreamWriter outputFile = new StreamWriter(propertiesFile))
        {
            await outputFile.WriteAsync(propertiesStringBuilder);
        }

        serverRepository.Save(server);
    }

    public async Task DeleteAsync(Server server, CancellationToken ct = default)
    {
        if (server.ServerDirectory is null) throw new NullReferenceException("Server directory cannot be null.");
        if (!Directory.Exists(server.ServerDirectory)) throw new IOException($"Directory '{server.ServerDirectory}' does not exist.");

        Directory.Delete(server.ServerDirectory, true);

        await serverRepository.DeleteAsync(server.Id);
    }

    public Task EditAsync(Server server, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

using System;
using Wave.Application.Middle;
using Wave.Application.Out.Minecraft;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Middle;

public class VersionManagerService : IVersionManagerService
{
    private IMinecraftVersionRepository minecraftVersionRepository;

    public VersionManagerService(IMinecraftVersionRepository minecraftVersionRepository)
    {
        this.minecraftVersionRepository = minecraftVersionRepository;
    }

    public async Task<Server> SetVersionAsync(Server server)
    {
        if (server.JarPath is null) throw new NullReferenceException("Server Jar path cannot be null.");
        if (server.Details.MinecraftVersion is null) throw new NullReferenceException("Requested Minecraft Version Cannot be null.");

        //Move old version if exists
        string oldJarPath = Path.Combine(server.Info.ServerDirectory!, "old.jar");
        if (File.Exists(server.JarPath))
        {
            File.Move(server.JarPath, oldJarPath, true);
        }

        try
        {
            server.Details.MinecraftVersionDetails = await minecraftVersionRepository.GetVersionDetailsAsync(server.Details.MinecraftVersion);
            await minecraftVersionRepository.DownloadMinecraftServer(server.Details.MinecraftVersionDetails, server.JarPath);
        }
        catch (Exception)
        {
            if (File.Exists(server.JarPath))
            {
                File.Move(oldJarPath, server.JarPath, true);
            }
        }

        return server;
    }
}

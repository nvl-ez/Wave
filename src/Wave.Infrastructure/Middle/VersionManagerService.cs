using System;
using Wave.Application.Middle;
using Wave.Application.Out.Minecraft;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Middle;

public class VersionManagerService : IVersionManagerService
{
    private readonly IMinecraftVersionRepository minecraftVersionRepository;
    private readonly IServerPathResolver serverPathResolver;

    public VersionManagerService(IServerPathResolver serverPathResolver, IMinecraftVersionRepository minecraftVersionRepository)
    {
        this.minecraftVersionRepository = minecraftVersionRepository;
        this.serverPathResolver = serverPathResolver;
    }

    public async Task<Server> SetVersionAsync(Server server)
    {
        //Move old version if exists
        string oldJarPath = Path.Combine(serverPathResolver.GetServerRootDirectory(server), "old.jar");
        string jarPath = serverPathResolver.GetServerJarPath(server);
        if (File.Exists(jarPath))
        {
            File.Move(jarPath, oldJarPath, true);
        }

        int? oldRequiredJavaVersion = server.JavaVersion;

        try
        {
            MinecraftVersionDetails details = await minecraftVersionRepository.GetVersionDetailsAsync(server.MinecraftVersionInfo);
            server.JavaVersion = details.JavaVersion;
            await minecraftVersionRepository.DownloadMinecraftServer(details, jarPath);
        }
        catch (Exception)
        {
            server.JavaVersion = oldRequiredJavaVersion;
            if (File.Exists(oldJarPath))
            {
                File.Move(oldJarPath, jarPath, true);
            }
        }

        return server;
    }
}

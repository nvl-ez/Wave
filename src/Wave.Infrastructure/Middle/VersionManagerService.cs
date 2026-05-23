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

    public async Task<Server> SetVersionAsync(Server server, ServerQuery serverQuery, CancellationToken ct = default)
    {
        if (
            serverQuery.MinecraftVersionBase is null ||
            string.Equals(server.MinecraftVersionInstallation?.MinecraftVersion, serverQuery.MinecraftVersionBase.MinecraftVersion, StringComparison.OrdinalIgnoreCase)
        ) return server;

        //Move old version if exists
        string oldJarPath = Path.Combine(serverPathResolver.GetServerRootDirectory(server), "old.jar");
        string jarPath = serverPathResolver.GetServerJarPath(server);
        if (File.Exists(jarPath))
        {
            File.Move(jarPath, oldJarPath, true);
        }

        MinecraftVersionInstallation? oldMinecraftVersionInstallation = server.MinecraftVersionInstallation;

        try
        {
            MinecraftVersionDetails details = await minecraftVersionRepository.GetVersionDetailsAsync((MinecraftVersionInfo)serverQuery.MinecraftVersionBase);
            server.MinecraftVersionInstallation = await minecraftVersionRepository.DownloadMinecraftServer(details, jarPath);
        }
        catch (Exception)
        {
            server.MinecraftVersionInstallation = oldMinecraftVersionInstallation;
            if (File.Exists(oldJarPath))
            {
                File.Move(oldJarPath, jarPath, true);
            }
        }

        return server;
    }
}

using System;
using Wave.Application.Middle;
using Wave.Application.Out.Modloader;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Infrastructure.Middle;

public class ModloaderManagerService : IModloaderManagerService
{
    private string serverTmpDirectory;
    private IEnumerable<IModloaderVersionCatalog> modloaders;

    public ModloaderManagerService(string serverTmpDirectory, IEnumerable<IModloaderVersionCatalog> modloaders)
    {
        this.modloaders = modloaders;
        this.serverTmpDirectory = serverTmpDirectory;
    }

    public async Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(ModloaderType modloaderType, MinecraftVersionInfo minecraftVersionInfo, CancellationToken ct = default)
    {
        IModloaderVersionCatalog? targetModloader = null;
        foreach (var modloader in modloaders)
        {
            if (modloader.CanHandleType(modloaderType))
            {
                targetModloader = modloader;
                break;
            }
        }

        if (targetModloader is null) throw new InvalidDataException($"There is no modloader that can handle the typ {modloaderType}.");//TODO: Fix error handling

        return await targetModloader.GetModloaderVersionsAsync(minecraftVersionInfo);
    }

    public Task<Server> AddModloaderAsync(Server server, ModloaderInfo modloaderInfo, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Server> RemoveModloaderAsync(Server server, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}

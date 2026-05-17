using System;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Application.Middle;

public interface IModloaderManagerService
{
    public Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(ModloaderType modloaderType, MinecraftVersionInfo minecraftVersionInfo, CancellationToken ct = default);
    public Task<Server> AddModloaderAsync(Server server, ModloaderInfo modloaderInfo, CancellationToken ct = default);
    public Task<Server> RemoveModloaderAsync(Server server, CancellationToken ct = default);
}

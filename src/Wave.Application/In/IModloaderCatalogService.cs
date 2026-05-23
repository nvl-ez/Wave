using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Application.In;

public interface IModloaderCatalogService
{
    public Task<IEnumerable<KeyValuePair<string, ModloaderType>>> GetModloaderTypesAsync(CancellationToken ct = default);
    public Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(ModloaderType modloaderType, string minecraftVersion, CancellationToken ct = default);
}

using System;
using Wave.Application.In;
using Wave.Application.Out.Modloader.Api;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;

namespace Wave.Application.Services;

public class ModloaderVersionCatalogService : IModloaderVersionCatalogService
{
    private readonly IModloaderVersionCatalog modloaderVersionCatalog;
    public ModloaderVersionCatalogService(IModloaderVersionCatalog modloaderVersionCatalog)
    {
        this.modloaderVersionCatalog = modloaderVersionCatalog;
    }
    public async Task<IEnumerable<ModloaderVersion>> GetModloaderVersionsAsync(MinecraftVersion minecraftVersion, CancellationToken ct)
    {
        return await modloaderVersionCatalog.GetModloaderVersionsAsync(minecraftVersion, ct);
    }
}

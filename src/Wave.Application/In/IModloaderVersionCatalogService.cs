using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;

namespace Wave.Application.In;

public interface IModloaderVersionCatalogService
{
    public Task<IEnumerable<ModloaderVersion>> GetModloaderVersionsAsync(MinecraftVersion minecraftVersion, CancellationToken ct);
}

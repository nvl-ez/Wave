using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;

namespace Wave.Application.Out.Modloader.Api;

public interface IModloaderVersionCatalog
{
    public Task<IEnumerable<ModloaderVersion>> GetModloaderVersionsAsync(MinecraftVersion minecraftVersion, CancellationToken ct);
}

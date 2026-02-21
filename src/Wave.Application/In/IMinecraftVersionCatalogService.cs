using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.In;

public interface IMinecraftVersionCatalogService
{
    public Task<IEnumerable<MinecraftVersion>> GetMinecraftVersionsAsync(bool includeSnapshots, CancellationToken ct);
}

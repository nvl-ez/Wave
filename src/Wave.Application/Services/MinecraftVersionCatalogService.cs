using System;
using Wave.Application.In;
using Wave.Application.Out.Minecraft.Api;
using Wave.Domain.Minecraft;

namespace Wave.Application.Services;

public class MinecraftVersionCatalogService : IMinecraftVersionCatalogService
{
    private readonly IMinecraftVersionCatalog minecraftVersionCatalog;

    public MinecraftVersionCatalogService(IMinecraftVersionCatalog minecraftVersionCatalog)
    {
        this.minecraftVersionCatalog = minecraftVersionCatalog;
    }

    public async Task<IEnumerable<MinecraftVersion>> GetMinecraftVersionsAsync(bool includeSnapshots, CancellationToken ct)
    {

        List<MinecraftVersion> versions = (await minecraftVersionCatalog.GetMinecraftVersionsAsync(ct))
        .Where(x => includeSnapshots || x.VersionType == MinecraftVersion.VersionTypeEnum.Release)
        .ToList();

        return versions;
    }

}

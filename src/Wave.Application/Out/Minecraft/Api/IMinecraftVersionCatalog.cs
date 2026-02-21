using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.Out.Minecraft.Api;

public interface IMinecraftVersionCatalog
{
    public Task<List<MinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken ct);
}

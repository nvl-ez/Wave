using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.Out.Minecraft;

public interface IMinecraftVersionRepository
{
    public Task<List<MinecraftVersion>> GetMinecraftVersionsAsync(CancellationToken ct = default);
}

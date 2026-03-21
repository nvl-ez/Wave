using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.Out.Minecraft;

public interface IMinecraftVersionRepository
{
    public Task<List<MinecraftVersion>> GetAllAsync(CancellationToken ct = default);
    public Task<MinecraftVersion> GetDetailsAsync(MinecraftVersion minecraftVersion, CancellationToken ct = default);
    public Task<MinecraftVersion> Download(MinecraftVersion minecraftVersion, string filename, string destination, CancellationToken ct = default);
}

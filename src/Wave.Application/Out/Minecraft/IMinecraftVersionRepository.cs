using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.Out.Minecraft;

public interface IMinecraftVersionRepository
{
    public Task<List<MinecraftVersion>> GetAllVersionsAsync(CancellationToken ct = default);
    public Task<MinecraftVersionDetails> GetVersionDetailsAsync(MinecraftVersion minecraftVersion, CancellationToken ct = default);
    public Task<string> DownloadMinecraftServer(MinecraftVersionDetails minecraftVersionDetails, string serverPath, CancellationToken ct = default);
}

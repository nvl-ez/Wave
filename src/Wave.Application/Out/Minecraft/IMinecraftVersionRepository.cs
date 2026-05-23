using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.Out.Minecraft;

public interface IMinecraftVersionRepository
{
    public Task<List<MinecraftVersionInfo>> GetAllVersionsAsync(CancellationToken ct = default);
    public Task<MinecraftVersionDetails> GetVersionDetailsAsync(MinecraftVersionInfo minecraftVersionInfo, CancellationToken ct = default);
    public Task<MinecraftVersionInstallation> DownloadMinecraftServer(MinecraftVersionDetails minecraftVersionDetails, string serverJarPath, CancellationToken ct = default);
}

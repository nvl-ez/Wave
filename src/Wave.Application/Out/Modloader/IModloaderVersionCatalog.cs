using System;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Application.Out.Modloader;

public interface IModloaderVersionCatalog
{
    public Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(MinecraftVersionInfo minecraftVersionInfo, CancellationToken ct = default);
    public Task<ModloaderPackage> DownloadModloaderAsync(ModloaderInfo modloaderInfo, string path, CancellationToken ct = default);
    public Task<ModloaderInstallation> InstallModloaderAsync(string targetDirectory, ModloaderPackage modloaderPackage, JavaInstallation javaInstallation, CancellationToken ct = default);
    public bool CanHandleType(ModloaderType type);
}

using System;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.Modloader;

public interface IModloaderVersionCatalog
{
    public Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(MinecraftVersion minecraftVersion, CancellationToken ct = default);
    public Task<ModloaderPackage> DownloadModloader(ModloaderInfo modloader, string path, CancellationToken ct = default);
    public Task<ModloaderInstallation> InstallModloader(string targetDirectory, ModloaderPackage modloaderPackage, JavaInstallation javaInstallation, CancellationToken ct = default);
}

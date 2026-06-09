using System;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Application.Out.Modloader;

public interface IModloaderVersionCatalog
{
    public ModloaderType ModloaderType { get; }
    public Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(string minecraftVersion, CancellationToken ct = default);
    public Task<ModloaderPackage> DownloadModloaderAsync(ModloaderInfo modloaderInfo, string filePath, CancellationToken ct = default);
    public Task<ModloaderInstallation> InstallModloaderAsync(string targetDirectory, ModloaderPackage modloaderPackage, JavaInstallation javaInstallation, CancellationToken ct = default);
    public bool CanHandleType(ModloaderType type);
}

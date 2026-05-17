using System;

namespace Wave.Domain.ServerManager.Modloader;

public class ModloaderInstallation
{
    public required ModloaderType Type { get; set; }
    public required string Version { get; set; }
    public required string MinecraftVersion { get; set; }
}

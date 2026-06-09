using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager.Modloader;

public class ModloaderPackage : ModloaderBase
{
    public required string InstallerPath { get; set; }
    public required string InstallerVersion { get; set; }
}

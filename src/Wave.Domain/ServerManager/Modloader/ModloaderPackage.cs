using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager.Modloader;

public class ModloaderPackage
{
    public required ModloaderType ModloaderType { get; set; }
    public required string InstallerPath { get; set; }
    public required string InstallerVersion { get; set; }
    public required string ModloaderVersion { get; set; }
    public required MinecraftVersionInfo MinecraftVersionInfo { get; set; }
}

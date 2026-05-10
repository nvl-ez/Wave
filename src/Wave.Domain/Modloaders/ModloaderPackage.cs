using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.Modloaders;

public class ModloaderPackage
{
    public required ModloaderType ModloaderType { get; set; }
    public required string InstallerPath { get; set; }
    public required string InstallerVersion { get; set; }
    public required string ModloaderVersion { get; set; }
    public required MinecraftVersion MinecraftVersion { get; set; }
}

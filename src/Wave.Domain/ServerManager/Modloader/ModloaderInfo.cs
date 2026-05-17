using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager.Modloader;

public class ModloaderInfo
{
    public required ModloaderType ModloaderType { get; set; }
    public required string Version { get; set; }
    public required MinecraftVersionInfo MinecraftVersionInfo { get; set; }
    public required string DowloadUrl { get; set; }
}

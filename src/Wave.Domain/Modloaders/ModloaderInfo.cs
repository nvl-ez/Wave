using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.Modloaders;

public class ModloaderInfo
{
    public required string Version { get; set; }
    public required MinecraftVersion MinecraftVersion { get; set; }
    public required string DowloadUrl { get; set; }
}

using System;

namespace Wave.Domain.Modloaders;

public abstract class ModloaderVersion
{
    public required string Version { get; set; }
    public required string MinecraftVersion { get; set; }
    public required string DowloadUrl { get; set; }
}

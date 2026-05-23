using System;

namespace Wave.Domain.Minecraft;

public class MinecraftVersionInstallation : MinecraftVersionBase
{
    public required int JavaVersion { get; set; }
}

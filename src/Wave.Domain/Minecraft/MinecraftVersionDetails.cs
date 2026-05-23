using System;

namespace Wave.Domain.Minecraft;

public class MinecraftVersionDetails : MinecraftVersionBase
{
    public required int JavaVersion { get; set; }
    public required string ServerUrl { get; set; }
}

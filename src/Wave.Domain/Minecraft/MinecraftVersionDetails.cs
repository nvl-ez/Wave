using System;

namespace Wave.Domain.Minecraft;

public class MinecraftVersionDetails
{
    public required int JavaVersion { get; set; }
    public required string ServerUrl { get; set; }
}

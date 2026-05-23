using System;

namespace Wave.Domain.Minecraft;

public sealed class MinecraftVersionInfo : MinecraftVersionBase
{
    public required string DetailsUrl { get; set; }
}

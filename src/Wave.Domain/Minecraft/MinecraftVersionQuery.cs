using System;

namespace Wave.Domain.Minecraft;

public record class MinecraftVersionQuery
{
    public bool IncludeSnapshots { get; set; } = false;
}

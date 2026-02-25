using System;

namespace Wave.Domain.Minecraft;

public record class MinecraftVersion
{
    public required string Version { get; set; }
    public required MinecraftVersionType MinecraftVersionType { get; set; }
    public string? DetailsUrl { get; set; }
    public required DateTime ReleaseDate { get; set; }
}

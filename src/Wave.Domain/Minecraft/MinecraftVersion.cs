using System;

namespace Wave.Domain.Minecraft;

public record class MinecraftVersion
{
    public required string Version { get; set; }
    public required VersionTypeEnum VersionType { get; set; }
    public Uri? DetailsUrl { get; set; }
    public required DateTime ReleaseDate { get; set; }

    public enum VersionTypeEnum
    {
        Release,
        Snapshot,
        Other
    }
}

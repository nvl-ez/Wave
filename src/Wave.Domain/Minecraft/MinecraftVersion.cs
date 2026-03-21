using System;

namespace Wave.Domain.Minecraft;

public sealed class MinecraftVersion
{
    public required string Version { get; set; }
    public required MinecraftVersionType MinecraftVersionType { get; set; }
    public string? DetailsUrl { get; set; } = null;
    public required DateTime ReleaseDate { get; set; }
    public int? JavaVersion { get; set; } = null;
    public string? ServerUrl { get; set; } = null;

    public bool Equals(MinecraftVersion? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return Version == other.Version;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Version);
    }
}

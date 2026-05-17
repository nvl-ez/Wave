using System;

namespace Wave.Domain.Minecraft;

public sealed class MinecraftVersionInfo
{
    public required string MinecraftVersion { get; set; }
    public required MinecraftVersionType MinecraftVersionType { get; set; }
    public required string DetailsUrl { get; set; }
    public required DateTime ReleaseDate { get; set; }

    public bool Equals(MinecraftVersionInfo? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return MinecraftVersion == other.MinecraftVersion;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinecraftVersion);
    }
}

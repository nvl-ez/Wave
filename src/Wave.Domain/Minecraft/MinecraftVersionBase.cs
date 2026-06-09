using System;

namespace Wave.Domain.Minecraft;

public abstract class MinecraftVersionBase
{
    public required string MinecraftVersion { get; set; }
    public required MinecraftVersionType MinecraftVersionType { get; set; }
    public required DateTime ReleaseDate { get; set; }

    public bool Equals(MinecraftVersionBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return MinecraftVersion == other.MinecraftVersion;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MinecraftVersionBase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinecraftVersion);
    }
}

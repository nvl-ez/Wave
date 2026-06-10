using System;

namespace Wave.Domain.ServerManager.Modloader;

public abstract class ModloaderBase
{
    public required ModloaderType ModloaderType { get; set; }
    public required string Version { get; set; }
    public required string MinecraftVersion { get; set; }

    public bool Equals(ModloaderBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return MinecraftVersion == other.MinecraftVersion && Version == other.Version && ModloaderType == other.ModloaderType;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ModloaderBase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinecraftVersion, Version, ModloaderType);
    }
}

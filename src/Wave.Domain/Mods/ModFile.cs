using System;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.Mods;

public class ModFile : ModBase, IEquatable<ModFile>
{
    public ModFile(ModInfo modInfo, ModVersion modVersion)
    {
        if (modInfo.ModSupplierType != modVersion.ModSupplierType) throw new InvalidDataException("Mod info and version must be of the same supplier");

        ModId = modInfo.ModId;
        Name = modInfo.Name;
        Summary = modInfo.Summary;
        IconUrl = modInfo.IconUrl;
        Slug = modInfo.Slug;

        VersionId = modVersion.VersionId;
        Version = modVersion.Version;
        ModSupplierType = modVersion.ModSupplierType;
        MinecraftVersion = modVersion.MinecraftVersion;
        ModVersionType = modVersion.ModVersionType;
        ModloaderType = modVersion.ModloaderType;
    }

    public string VersionId { get; set; }
    public string Version { get; set; }
    public string MinecraftVersion { get; set; }
    public ModVersionType ModVersionType { get; set; }
    public ModloaderType ModloaderType { get; set; }

    public bool Equals(ModFile? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return ModId == other.ModId &&
               VersionId == other.VersionId;
    }

    public override bool Equals(object? obj)
    {
        return obj is ModFile other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), ModId, VersionId);
    }
}

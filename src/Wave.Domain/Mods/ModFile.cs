using System;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.Mods;

public class ModFile : ModBase, IEquatable<ModFile>
{
    public ModFile()
    {
        VersionName = string.Empty;
        VersionId = string.Empty;
        Version = string.Empty;
        MinecraftVersion = string.Empty;
        Artifacts = [];
    }
    public ModFile(ModBase modBase, ModVersion modVersion) : base(modBase.ModId, modBase.ModName, modBase.Slug, modBase.ModSupplierType, modBase.ModSummary, modBase.IconUrl)
    {
        if (modBase.ModSupplierType != modVersion.ModSupplierType) throw new InvalidDataException("Mod info and version must be of the same supplier");

        VersionId = modVersion.VersionId;
        Version = modVersion.Version;
        ModSupplierType = modVersion.ModSupplierType;
        MinecraftVersion = modVersion.MinecraftVersion;
        ModVersionType = modVersion.ModVersionType;
        ModloaderType = modVersion.ModloaderType;
        VersionName = modVersion.VersionName;
        Artifacts = modVersion.Artifacts;
    }

    public string VersionName { get; set; }
    public string VersionId { get; set; }
    public string Version { get; set; }
    public string MinecraftVersion { get; set; }
    public IEnumerable<ModArtifact> Artifacts { get; set; }

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

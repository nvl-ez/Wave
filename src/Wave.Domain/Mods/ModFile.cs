using System;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.Mods;

public record class ModFile
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

    public string ModId { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public string? IconUrl { get; set; }
    public string Slug { get; set; }

    public string VersionId { get; set; }
    public string Version { get; set; }
    public ModSupplierType ModSupplierType { get; set; }
    public string MinecraftVersion { get; set; }
    public ModVersionType ModVersionType { get; set; }
    public ModloaderType ModloaderType { get; set; }
}

using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;

namespace Wave.Domain.Mods;

public record class ModVersion
{
    public required string Name { get; set; }
    public string Version { get; set; } = "";
    public required string ModId { get; set; }
    public required string VersionId { get; set; }
    public required List<ModArtifact> Artifacts { get; set; }
    public List<ModDependency>? Dependencies { get; set; }
    public string? Changelog { get; set; }
    public required MinecraftVersion MinecraftVersion { get; set; }
    public required ModVersionType ModVersionType { get; set; }
    public required ModSupplierType ModSupplierType { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public bool Featured { get; set; } = false;
}

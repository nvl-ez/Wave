using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;

namespace Wave.Domain.Mods;

public record class ModInfo
{
    public required ModSupplierType ModSupplierType { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? IconUrl { get; set; }
    public required MinecraftVersion MinecraftVersion { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public required string ModId { get; set; }
    public required string Slug { get; set; }
}

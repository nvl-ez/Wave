using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.Mods;

public record class ModInfo
{
    public required ModSupplierType ModSupplierType { get; set; }
    public required string Name { get; set; }
    public required string Summary { get; set; }
    public string? IconUrl { get; set; }
    public required string ModId { get; set; }
    public required string Slug { get; set; }
}

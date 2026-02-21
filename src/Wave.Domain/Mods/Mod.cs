using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Domain.ModSupplier;

namespace Wave.Domain.Mods;

public class Mod
{
    public required ModSupplierType ModSupplierType { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Uri? IconUrl { get; set; }
    public string? Version { get; set; }
    public required MinecraftVersion MinecraftVersion { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public string? ExternalId { get; set; }
    public Uri? DownloadUrl;
}

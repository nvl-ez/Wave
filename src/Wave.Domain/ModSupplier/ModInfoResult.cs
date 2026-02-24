using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;

namespace Wave.Domain.ModSupplier;

public record class ModInfoResult
{
    public required ModSupplierType ModSupplierType { get; set; }
    public required string Name { get; set; }
    public Uri? IconUrl { get; set; }
    public required MinecraftVersion MinecraftVersion { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public required string ExternalId { get; set; }
    public required string Slug { get; set; }
}

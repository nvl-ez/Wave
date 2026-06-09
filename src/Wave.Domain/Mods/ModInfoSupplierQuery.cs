using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;
using Wave.Domain.Utils;

namespace Wave.Domain.Mods;

public record class ModInfoSupplierQuery
{
    public string? TextQuery { get; set; }
    public string? Author { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public required string MinecraftVersion { get; set; }
    public required PaginationState PaginationState { get; set; }
    public required ModSupplierType ModSupplierType { get; set; }
}

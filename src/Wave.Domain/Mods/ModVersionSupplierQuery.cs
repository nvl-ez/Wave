using System;
using Wave.Domain.ServerManager.Modloader;
using Wave.Domain.Utils;

namespace Wave.Domain.Mods;

public class ModVersionSupplierQuery
{
    public required string ModId { get; set; }
    public required string MinecraftVersion { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public required ModSupplierType ModSupplierType { get; set; }
    public PaginationState? PaginationState { get; set; }
}

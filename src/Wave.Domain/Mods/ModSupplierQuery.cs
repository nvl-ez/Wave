using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.Mods;

public record class ModSupplierQuery
{
    public string? TextQuery { get; set; }
    public string? Author { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public required string MinecraftVersion { get; set; }
    public int Offset { get; set; } = 0;
    public int PageSize { get; set; } = 20;
}

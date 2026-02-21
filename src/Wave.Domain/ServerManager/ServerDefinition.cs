using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Domain.Mods;

namespace Wave.Domain.ServerManager;

public class ServerDefinition
{
    public required Guid Id { get; set; } = Guid.NewGuid();
    public required DateTime CreationDate { get; set; } = DateTime.Now;
    public required MinecraftVersion MinecraftVersion { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public ModloaderVersion? ModloaderVersion { get; set; } = null;
    public List<Mod>? Mods { get; set; } = null;
}

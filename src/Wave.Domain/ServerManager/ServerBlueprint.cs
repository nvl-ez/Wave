using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Domain.Mods;

namespace Wave.Domain.ServerManager;

public class ServerRequest
{
    public required DateTime CreationDate { get; set; } = DateTime.Now;
    public required MinecraftVersion MinecraftVersion { get; set; }
    public required ModloaderType ModloaderType { get; set; }
    public ModloaderVersion? ModloaderVersion { get; set; } = null;
    public List<ModVersion>? Mods { get; set; } = null;
}

using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager.Modloader;

public class ModloaderInfo : ModloaderBase
{
    public required string DowloadUrl { get; set; }
}

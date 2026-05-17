using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.ServerManager;

public record class ServerCreationQuery
{
    public required string Name { get; set; }
    public required MinecraftVersionInfo MinecraftVersionInfo { get; set; }
    public required bool Eula { get; set; }
    public required Dictionary<string, string> Properties { get; set; }
    public required ModloaderInfo? ModloaderInfo { get; set; }
}

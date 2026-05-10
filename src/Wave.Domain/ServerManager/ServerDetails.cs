using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager;

public sealed class ServerDetails
{
    public MinecraftVersion? MinecraftVersion { get; set; }
    public MinecraftVersionDetails? MinecraftVersionDetails { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new()
    {
        {"difficulty", "nromal"},
        {"gamemode", "survival"},
        {"level-seed", ""},
        {"max-players", "16"},
        {"motd", "A Minecraft Server"},
        {"online-mode", "true"},
        {"server-ip", ""},
        {"spawn-protection", "16"},
        {"view-distance", "8"}
    };

    public string ServerFilename { get; set; } = "server.jar";
    public string PropertiesFilename { get; set; } = "server.properties";
    public string EulaFilename { get; set; } = "eula.txt";
    public bool Eula { get; set; } = false;
}

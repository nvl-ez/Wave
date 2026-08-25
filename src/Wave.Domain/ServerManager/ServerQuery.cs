using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Mods;
using Wave.Domain.Java;
using Wave.Domain.ServerManager.Modloader;
using System.Text.Json.Serialization;

namespace Wave.Domain.ServerManager;

public record class ServerQuery
{
    public ServerQuery() { }
    public ServerQuery(Server server)
    {
        Id = server.Id;
        Name = server.Name;
        ExecutionFlags = server.ExecutionFlags;
        JavaInstallation = server.JavaInstallation;
        MinecraftVersionBase = server.MinecraftVersionInstallation;
        Eula = server.Eula;
        Properties = server.Properties;
        CreationDate = server.CreationDate;
        Modloader = server.Modloader;
        Mods = server.Mods;
    }
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string ExecutionFlags { get; set; } = string.Empty;
    public JavaInstallation? JavaInstallation { get; set; }
    public bool IsJavaInstallationLocked { get; set; }
    public MinecraftVersionBase? MinecraftVersionBase { get; set; }
    public bool Eula { get; set; } = false;
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
        {"view-distance", "8"},
        {"server-port", "25565"}
    };
    public ModloaderBase? Modloader { get; set; }
    public DateTime? CreationDate { get; set; }
    public IEnumerable<ModFile> Mods { get; set; } = [];
}

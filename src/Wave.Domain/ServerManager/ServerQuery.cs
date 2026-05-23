using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.ServerManager;

public record class ServerQuery
{
    public ServerQuery() { }
    public ServerQuery(Server server)
    {
        Id = server.Id;
        Name = server.Name;
        MinecraftVersionBase = server.MinecraftVersionInstallation;
        Eula = server.Eula;
        Properties = server.Properties;
        CreationDate = server.CreationDate;
        Modloader = server.Modloader is not null ? new ModloaderInfo()
        {
            DowloadUrl = "",
            MinecraftVersion = server.MinecraftVersionInstallation!.MinecraftVersion,
            ModloaderType = server.Modloader.Type,
            Version = server.Modloader.Version
        } : null;
    }
    public Guid? Id { get; set; }
    public string? Name { get; set; }
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
        {"view-distance", "8"}
    };
    public ModloaderInfo? Modloader { get; set; }
    public DateTime? CreationDate { get; set; }
}

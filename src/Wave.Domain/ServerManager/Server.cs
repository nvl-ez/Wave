using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager;

public sealed class Server
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public DateTime CreationDate { get; init; } = DateTime.Now;
    public required MinecraftVersionInfo MinecraftVersionInfo { get; set; }
    public required int? JavaVersion { get; set; }

    /*******************
    * Physical Objects *
    *******************/
    public ServerPaths ServerPaths = new();
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
    public bool Eula { get; set; } = false;
    public Modloader.ModloaderInstallation? Modloader { get; set; } = null;
    public string? ImageFilename { get; set; } = null;

    public bool Equals(Server? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}

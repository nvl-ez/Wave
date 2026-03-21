using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager;

public sealed class Server
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public MinecraftVersion? MinecraftVersion { get; set; }
    public Dictionary<string, string?> Properties { get; set; } = new()
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
    public string? ServerDirectory { get; set; } = null;
    public string? ServerFilename { get; set; } = null;
    public string? ServerPropertiesFilename { get; set; } = null;

    public bool IsReady => ServerDirectory is not null &&
        MinecraftVersion is not null &&
        ServerFilename is not null &&
        ServerPropertiesFilename is not null;

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

using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager;

public sealed class Server
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public MinecraftVersion? MinecraftVersion { get; set; }
    public Dictionary<string, string> Properties = new()
    {
        {"difficulty", "Difficulty"},
        {"gamemode", "Gamemode"},
        {"level-seed", "Level Seed"},
        {"max-players", "Max Players"},
        {"motd", "Motd"},
        {"online-mode", "Online Mode"},
        {"server-ip", "Server IP"},
        {"spawn-protection", "Spawn Protection"},
        {"view-distance", "View Distance"}
    };

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

using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager;

public sealed class Server
{
    public ServerInfo Info { get; set; } = new();
    public ServerDetails Details { get; set; } = new();

    public Guid Id => Info.Id;

    public bool IsReady => Info.ServerDirectory is not null &&
        Details.MinecraftVersion is not null &&
        Details.ServerFilename is not null &&
        Details.PropertiesFilename is not null;

    public bool Equals(Server? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return Info.Id == other.Info.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Info.Id);
    }
}

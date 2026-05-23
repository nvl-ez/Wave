using System;
using Wave.Domain.Minecraft;

namespace Wave.Domain.ServerManager;

public sealed class Server
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public DateTime CreationDate { get; init; } = DateTime.Now;
    public MinecraftVersionInstallation? MinecraftVersionInstallation { get; set; }


    /*******************
    * Physical Objects *
    *******************/
    public ServerPaths ServerPaths = new();
    public required Dictionary<string, string> Properties { get; set; }
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

using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Mods;
using Wave.Domain.Java;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.ServerManager;

public sealed class Server
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string ExecutionFlags { get; set; } = string.Empty;
    public JavaInstallation? JavaInstallation { get; set; }
    public DateTime CreationDate { get; init; } = DateTime.Now;
    public ServerPaths ServerPaths = new();



    /*******************
    * Physical Objects *
    *******************/
    public MinecraftVersionInstallation? MinecraftVersionInstallation { get; set; }
    public required Dictionary<string, string> Properties { get; set; }
    public bool Eula { get; set; } = false;
    public ModloaderInstallation? Modloader { get; set; } = null;
    public string? ImageFilename { get; set; } = null;
    public IEnumerable<ModFile> Mods { get; set; } = [];

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

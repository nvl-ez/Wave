using System;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.ServerManager;

public class ServerChanges
{
    public IEnumerable<ModFile>? DeletedMods { get; set; } = null;

    public ModloaderBase? DeletedModloader { get; set; } = null;
}

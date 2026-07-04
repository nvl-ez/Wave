using System;
using Wave.Domain.Mods;

namespace Wave.Domain.ServerManager;

public class ServerChanges
{
    public IEnumerable<ModFile>? DeletedMods { get; set; }

    public bool? MigratedModloader { get; set; } = null; //True if migrated. False if deleted. Null if nothing.
}

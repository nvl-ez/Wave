using System;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;

namespace Wave.Application.Middle;

public interface IModManagerService
{
    public Task SetModsAsync(Server server, ServerQuery query);
    public Task<IEnumerable<ModFile>> MigrateModsAsync(Server server);
}

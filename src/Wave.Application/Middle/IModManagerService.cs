using System;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;

namespace Wave.Application.Middle;

public interface IModManagerService
{
    public Task<ModMigrationResult> SetModsAsync(Server server, ServerQuery query);
    public Task<ModMigrationResult> MigrateModsAsync(Server server);
}

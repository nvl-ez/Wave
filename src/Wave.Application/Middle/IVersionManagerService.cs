using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;

namespace Wave.Application.Middle;

public interface IVersionManagerService
{
    public Task<Server> SetVersionAsync(Server server, ServerQuery serverQuery, CancellationToken ct = default);
}

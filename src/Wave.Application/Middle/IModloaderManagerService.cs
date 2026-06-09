using System;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Application.Middle;

public interface IModloaderManagerService
{
    public Task<Server> AddModloaderAsync(Server server, ServerQuery query, CancellationToken ct = default);
    public Task<Server> RemoveModloaderAsync(Server server, CancellationToken ct = default);
}

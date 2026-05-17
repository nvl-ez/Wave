using System;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerExecutor
{
    public IServerSession Start(Guid serverId, string serverDirectory, string jarPath, JavaInstallation javaInstallation, CancellationToken ct = default);
}

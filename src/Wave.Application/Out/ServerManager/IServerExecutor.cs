using System;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerExecutor
{
    public IServerSession Start(Server server, IJavaInstallation javaInstallation, CancellationToken ct = default);
}

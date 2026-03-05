using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaInstaller
{
    public Task<JavaInstallation> Install(JavaVersion javaVersion, string destinationDirectory, CancellationToken ct);
    public Task<bool> Uninstall(JavaInstallation javaInstallation, CancellationToken ct);
}

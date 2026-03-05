using System;
using Wave.Application.Out.Java;
using Wave.Domain.Java;

namespace Wave.Infrastructure.Out.Java.Installer;

public class ExecutableInstaller : IJavaInstaller
{
    public Task<JavaInstallation> Install(JavaVersion javaVersion, string destinationDirectory, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Uninstall(JavaInstallation javaInstallation, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

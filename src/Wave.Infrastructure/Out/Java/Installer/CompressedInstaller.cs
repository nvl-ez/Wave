using System;
using Wave.Application.Out.Java;
using Wave.Domain.Java;

namespace Wave.Infrastructure.Out.Java.Installer;

public class CompressedInstaller : IJavaInstaller
{
    public Task<JavaInstallation?> Install(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Uninstall(JavaInstallation javaInstallation, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaInstaller
{
    public Task<JavaInstallation?> Install(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct = default);
    public Task<bool> Uninstall(JavaInstallation javaInstallation, CancellationToken ct = default);
}

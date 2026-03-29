using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaInstaller<in TPackage> : IJavaInstaller
    where TPackage : IJavaPackage
{
    public JavaInstallation Install(TPackage javaPackage, CancellationToken ct = default);
}

public interface IJavaInstaller
{
    public bool CanInstall(IJavaPackage javaPackage);
    public bool CanUninstall(JavaInstallation javaInstallation);
    JavaInstallation Install(IJavaPackage javaPackage, CancellationToken ct = default);
    void Uninstall(JavaInstallation javaInstallation, CancellationToken ct = default);
}
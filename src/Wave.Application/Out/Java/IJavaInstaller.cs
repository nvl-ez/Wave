using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaInstaller<in TPackage, TInstallation>
    where TPackage : IJavaPackage
    where TInstallation : IJavaInstallation
{
    public TInstallation Install(TPackage javaPackage, CancellationToken ct = default);
    public void Unistall(TInstallation javaInstallation, CancellationToken ct = default);
    public bool CanInstall(IJavaPackage javaPackage);
}

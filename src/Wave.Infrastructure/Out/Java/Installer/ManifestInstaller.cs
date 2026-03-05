using System;
using Wave.Application.In;
using Wave.Application.Out.Java;
using Wave.Domain.Java;

namespace Wave.Infrastructure.Out.Java.Installer;

public class ManifestInstaller : IJavaInstaller
{
    public Task<JavaInstallation> Install(JavaVersion javaVersion)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Uninstall(JavaInstallation javaInstallation)
    {
        throw new NotImplementedException();
    }
}

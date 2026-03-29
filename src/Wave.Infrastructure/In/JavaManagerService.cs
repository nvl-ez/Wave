using System;
using Wave.Application.In;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.System;
using Wave.Infrastructure.Exceptions;

namespace Wave.Infrastructure.In;

public class JavaManagerService : IJavaManagerService
{
    private IJavaInstallRepository javaInstallRepository;
    private List<IJavaSupplier> javaSuppliers;
    private List<IJavaInstaller> javaInstallers;

    public JavaManagerService(IJavaInstallRepository javaInstallRepository, List<IJavaSupplier> javaSuppliers, List<IJavaInstaller> javaInstallers)
    {
        this.javaInstallRepository = javaInstallRepository;
        this.javaSuppliers = javaSuppliers;
        this.javaInstallers = javaInstallers;
    }

    public async Task<IEnumerable<int>> GetAvailableMajorVersionsAsync(IJavaSupplier javaSupplier, OsType? os = null, CancellationToken ct = default)
    {
        return await javaSupplier.GetAvailableMajorVersionsAsync(os);
    }

    public async Task<IEnumerable<JavaInstallation>> GetJavaInstallationsAsync(CancellationToken ct = default)
    {
        return await javaInstallRepository.GetAllAsync();
    }

    public IEnumerable<IJavaSupplier> GetJavaSuppliers() //TODO: Abstraer la info del supplier
    {
        return javaSuppliers;
    }

    public async Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(IJavaSupplier javaSupplier, JavaSupplierQuery javaSupplierQuery, CancellationToken ct = default) //TODO: pasar supplier por ID
    {
        return await javaSupplier.GetJavaVersionsAsync(javaSupplierQuery);
    }

    public async Task<JavaInstallation> InstallJavaVersionAsync(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct = default)
    {
        IJavaPackage? javaPackage = null;
        foreach (var supplier in javaSuppliers)
        {
            if (supplier.CanDownload(javaVersion))
            {
                javaPackage = await supplier.DownloadJavaAsync(javaVersion, javaArtifact);
                break;
            }
        }
        if (javaPackage is null) throw new JavaInstallationException("The downloaded Java Package is null.");

        JavaInstallation? javaInstallation = null;
        foreach (var installer in javaInstallers)
        {
            if (installer.CanInstall(javaPackage))
            {
                javaInstallation = installer.Install(javaPackage);
                break;
            }
        }
        if (javaInstallation is null) throw new JavaInstallationException("Java could not be installed.");

        await javaInstallRepository.AddAsync(javaInstallation);

        return javaInstallation;
    }

    public async Task UninstallJavaArtifactAsync(JavaInstallation javaInstallation, CancellationToken ct = default)
    {
        foreach (var installer in javaInstallers)
        {
            if (installer.CanUninstall(javaInstallation))
            {
                installer.Uninstall(javaInstallation);
                break;
            }
        }

        await javaInstallRepository.RemoveAsync(javaInstallation);
    }
}

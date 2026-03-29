using System;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.System;

namespace Wave.Application.In;

public interface IJavaManagerService
{
    public Task<IEnumerable<JavaInstallation>> GetJavaInstallationsAsync(CancellationToken ct = default);
    public IEnumerable<IJavaSupplier> GetJavaSuppliers();
    public Task<IEnumerable<int>> GetAvailableMajorVersionsAsync(IJavaSupplier javaSupplier, OsType? os = null, CancellationToken ct = default);
    public Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(IJavaSupplier javaSupplier, JavaSupplierQuery javaSupplierQuery, CancellationToken ct = default);
    public Task<JavaInstallation> InstallJavaVersionAsync(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct = default);
    public Task UninstallJavaArtifactAsync(JavaInstallation javaInstallation, CancellationToken ct = default);

}

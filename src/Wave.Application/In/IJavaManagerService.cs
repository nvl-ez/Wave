using System;
using Wave.Application.Out.Java;
using Wave.Domain.Java;

namespace Wave.Application.In;

public interface IJavaManagerService
{
    public Task<IEnumerable<IJavaInstallation>> GetJavaInstallationsAsync();
    public IEnumerable<IJavaSupplier> GetJavaSuppliers();
    public Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(IJavaSupplier javaSupplier, JavaSupplierQuery javaSupplierQuery);
    public Task<JavaArtifact> InstallJavaVersionAsync(JavaVersion javaVersion);
    public Task UninstallJavaArtifactAsync(JavaArtifact javaArtifact);
}

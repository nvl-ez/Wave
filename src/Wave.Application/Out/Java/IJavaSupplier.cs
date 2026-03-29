using System;
using Wave.Domain.Java;
using Wave.Domain.System;

namespace Wave.Application.Out.Java;

public interface IJavaSupplier
{
    public string Name { get; set; }
    public Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery query, CancellationToken ct = default);
    public Task<IEnumerable<int>> GetAvailableMajorVersionsAsync(OsType? os = null, CancellationToken ct = default);
    public bool CanDownload(JavaVersion javaVersion);
    public Task<IJavaPackage> DownloadJavaAsync(JavaVersion javaVersion, JavaArtifact javaArtifact, CancellationToken ct = default);
}

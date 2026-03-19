using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaInstallRepository
{
    public Task<IEnumerable<JavaInstallation>> GetInstalledAsync(CancellationToken ct = default);
    public Task AddAsync(JavaInstallation javaInstallation, CancellationToken ct = default);
    public Task RemoveAsync(JavaInstallation javaInstallation, CancellationToken ct = default);
}

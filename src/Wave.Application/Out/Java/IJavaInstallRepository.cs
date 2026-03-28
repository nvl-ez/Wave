using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaInstallRepository
{
    public Task<IEnumerable<IJavaInstallation>> GetInstalledAsync(CancellationToken ct = default);
    public Task AddAsync(IJavaInstallation javaInstallation, CancellationToken ct = default);
    public Task RemoveAsync(IJavaInstallation javaInstallation, CancellationToken ct = default);
}

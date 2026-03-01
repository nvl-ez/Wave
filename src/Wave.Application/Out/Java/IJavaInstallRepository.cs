using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaInstallRepository
{
    public Task<IEnumerable<JavaInstallation>> GetInstalledAsync();
    public Task AddAsync(JavaInstallation javaInstallation);
    public Task RemoveAsync(JavaInstallation javaInstallation);
}

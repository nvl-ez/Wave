using System;
using Wave.Domain.Java;

namespace Wave.Application.Out.Java;

public interface IJavaSupplier
{
    public Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery? query, CancellationToken ct);
}

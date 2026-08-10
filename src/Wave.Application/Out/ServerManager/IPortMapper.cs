using Wave.Domain.System;

namespace Wave.Application.Out.ServerManager;

public interface IPortMapper
{
    Task<PortMappingLease> OpenAsync(int port, CancellationToken ct = default);
}

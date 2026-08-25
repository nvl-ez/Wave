using SharpOpenNat;
using Wave.Application.Out.ServerManager;
using Wave.Domain.System;

namespace Wave.Infrastructure.Out.ServerManager;

public sealed class SharpOpenNatPortMapper : IPortMapper
{
    private static readonly PortMapper[] supportedProtocols = [PortMapper.Upnp, PortMapper.Pmp];
    private readonly TimeSpan discoveryTimeout;

    public SharpOpenNatPortMapper(TimeSpan? discoveryTimeout = null)
    {
        this.discoveryTimeout = discoveryTimeout ?? TimeSpan.FromSeconds(15);
    }

    public async Task<PortMappingLease> OpenAsync(int port, CancellationToken ct = default)
    {
        if (port is < 1 or > 65535) throw new ArgumentOutOfRangeException(nameof(port));

        List<Exception> errors = [];
        foreach (PortMapper protocol in supportedProtocols)
        {
            try
            {
                INatDevice device;
                using (CancellationTokenSource timeout = CreateOperationTimeout(ct))
                {
                    device = await OpenNat.Discoverer.DiscoverDeviceAsync(protocol, timeout.Token);
                }

                Mapping requested = new(Protocol.Tcp, port, port, "Wave Minecraft server");

                Mapping? existing = null;
                if (protocol == PortMapper.Upnp)
                {
                    using CancellationTokenSource timeout = CreateOperationTimeout(ct);
                    existing = await device.GetSpecificMappingAsync(Protocol.Tcp, port, timeout.Token);
                }

                if (existing is not null
                    && existing.PrivatePort == port
                    && (existing.PrivateIP is null || existing.PrivateIP.Equals(device.LocalAddress)))
                {
                    return new PortMappingLease(port, () => ValueTask.CompletedTask);
                }

                using (CancellationTokenSource timeout = CreateOperationTimeout(ct))
                {
                    await device.CreatePortMapAsync(requested, timeout.Token);
                }

                return new PortMappingLease(
                    port,
                    async () => await device.DeletePortMapAsync(requested, CancellationToken.None));
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        }

        throw new AggregateException($"Could not map TCP port {port} using UPnP IGD or NAT-PMP.", errors);
    }

    private CancellationTokenSource CreateOperationTimeout(CancellationToken ct)
    {
        CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(discoveryTimeout);
        return timeout;
    }
}

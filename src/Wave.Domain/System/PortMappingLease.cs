namespace Wave.Domain.System;

public sealed class PortMappingLease : IAsyncDisposable
{
    private readonly Func<ValueTask> releaseAsync;
    private int disposed;

    public int Port { get; }

    public PortMappingLease(int port, Func<ValueTask> releaseAsync)
    {
        if (port is < 1 or > 65535) throw new ArgumentOutOfRangeException(nameof(port));

        Port = port;
        this.releaseAsync = releaseAsync ?? throw new ArgumentNullException(nameof(releaseAsync));
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref disposed, 1) != 0) return;
        await releaseAsync();
    }
}

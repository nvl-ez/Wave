using System;
using System.ComponentModel;

namespace Wave.Domain.ServerManager;

public interface IServerSession : IAsyncDisposable
{
    bool IsRunning { get; }

    Task SendCommandAsync(string command, CancellationToken ct = default);
    IAsyncEnumerable<string> GetOutputAsync(CancellationToken ct = default);

    event EventHandler<Guid>? ServerDisposed;

}

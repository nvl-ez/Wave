using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Out.ServerManager.Executor;

public class ServerSession : IServerSession
{
    private Process process;
    private Guid id;
    private CancellationTokenSource cts = new();
    private Channel<string> channel;

    private Task readStdoutTask;
    private Task readStderrTask;

    public event EventHandler<Guid>? ServerDisposed;

    public bool IsRunning => process is not null && !process.HasExited;

    public ServerSession(Process process, Guid id)
    {
        this.process = process ?? throw new NullReferenceException("Server Process cannot be null.");
        this.id = id;

        channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false
        });

        readStdoutTask = PumpAsync(process.StandardOutput, cts.Token);
        readStderrTask = PumpAsync(process.StandardError, cts.Token);
    }

    public async IAsyncEnumerable<string> GetOutputAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var line in channel.Reader.ReadAllAsync(ct))
        {
            yield return line;
        }
    }

    public async Task SendCommandAsync(string command, CancellationToken ct)
    {
        if (process.HasExited)
            throw new InvalidOperationException("The process has already exited.");

        await process.StandardInput.WriteLineAsync(command);
        await process.StandardInput.FlushAsync();
    }

    public async ValueTask DisposeAsync()
    {
        cts.Cancel();

        if (!process.HasExited)
            process.Kill();

        await Task.WhenAll(readStdoutTask, readStderrTask);

        channel.Writer.TryComplete();

        cts.Dispose();
        process.Dispose();

        ServerDisposed?.Invoke(this, id);
    }

    private async Task PumpAsync(StreamReader reader, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();

                Console.WriteLine(line);

                if (line == null)
                    break;

                await channel.Writer.WriteAsync(line, ct);
            }
        }
        finally
        {
            if (process.HasExited)
            {
                channel.Writer.TryComplete();
            }
        }
    }
}

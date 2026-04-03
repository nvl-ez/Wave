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
    private readonly int bufferSize = 10;

    private string[] buffer;
    private readonly Lock bufferLock = new();
    private CancellationTokenSource cts = new();
    private TaskCompletionSource<bool> signal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly Lock disposeLock = new();
    private Task? disposeTask;
    private long bufferIndex = 0;
    private int activePumps = 0;
    private bool completed = false;

    private Task readStdoutTask;
    private Task readStderrTask;

    public event EventHandler<Guid>? ServerDisposed;

    public bool IsRunning => process is not null && !completed && !process.HasExited;

    public ServerSession(Process process, Guid id)
    {
        this.process = process ?? throw new NullReferenceException("Server Process cannot be null.");
        this.id = id;
        process.Exited += OnProcessExited;

        buffer = new string[bufferSize];
        Array.Fill<string>(buffer, "");

        readStdoutTask = PumpAsync(process.StandardOutput, cts.Token);
        readStderrTask = PumpAsync(process.StandardError, cts.Token);
    }

    public async IAsyncEnumerable<string> GetOutputAsync([EnumeratorCancellation] CancellationToken ct = default)
    {
        long previousIndex = 0; //Exclusive (uno por delante)
        Queue<string> currentBuffer = new Queue<string>();
        while (!ct.IsCancellationRequested)
        {
            long currentIndex; //Exclusive
            currentBuffer.Clear();
            Task<bool> waitProduced;
            bool shouldBreak = false;

            //Obtain values that will be yielded
            lock (bufferLock)
            {
                currentIndex = bufferIndex;
                long unreadedLines = currentIndex - previousIndex;
                long unreadedBuffer = Math.Min(bufferSize, unreadedLines);

                for (long i = currentIndex - unreadedBuffer; i < currentIndex; i++)
                {
                    string line = buffer[i % bufferSize];
                    if (line != "")
                    {
                        currentBuffer.Enqueue(line);
                    }
                }

                waitProduced = signal.Task;
                if (completed) shouldBreak = true;
            }

            //Yield values
            foreach (string line in currentBuffer)
            {
                yield return line;
            }
            previousIndex = currentIndex;

            if (shouldBreak) yield break;

            //Check if new values can be yielded or await
            bool waitForValues;
            lock (bufferLock)
            {
                waitForValues = currentIndex >= bufferIndex;
            }
            if (waitForValues) await waitProduced;
        }
    }

    public async Task SendCommandAsync(string command, CancellationToken ct = default)
    {
        if (process.HasExited)
            throw new InvalidOperationException("The process has already exited.");

        await process.StandardInput.WriteLineAsync(command);
        await process.StandardInput.FlushAsync();
    }

    public ValueTask DisposeAsync()
    {
        Task task;

        lock (disposeLock)
        {
            disposeTask ??= DisposeCoreAsync();
            task = disposeTask;
        }

        return new ValueTask(task);
    }

    private async Task PumpAsync(StreamReader reader, CancellationToken ct)
    {
        //Increase pump counter
        lock (bufferLock)
        {
            activePumps++;
        }

        try
        {
            while (!ct.IsCancellationRequested)
            {
                //Await until server send produces a line
                TaskCompletionSource<bool> produced;
                var line = await reader.ReadLineAsync();

                //Server stopped
                if (line == null)
                    break;

                //Add new line into the buffer
                lock (bufferLock)
                {
                    buffer[bufferIndex % bufferSize] = line;
                    bufferIndex++;
                    produced = signal;
                    signal = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                }
                //Notify production
                produced.TrySetResult(true);
            }
        }
        finally
        {
            TaskCompletionSource<bool>? toRelease = null;

            lock (bufferLock)
            {
                activePumps--;

                if (activePumps == 0)
                {
                    completed = true;
                    toRelease = signal;
                }
            }

            // Wake consumers so they can observe completion
            toRelease?.TrySetResult(true);
        }
    }

    private async Task DisposeCoreAsync()
    {
        process.Exited -= OnProcessExited;
        cts.Cancel();

        try
        {
            if (!process.HasExited)
            {
                await SendCommandAsync("stop");

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                try
                {
                    await process.WaitForExitAsync(timeoutCts.Token);
                }
                catch (OperationCanceledException)
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                        await process.WaitForExitAsync();
                    }
                }
            }

            await Task.WhenAll(readStdoutTask, readStderrTask);
        }
        finally
        {
            cts.Dispose();
            process.Dispose();
            ServerDisposed?.Invoke(this, id);
        }
    }

    private async void OnProcessExited(object? sender, EventArgs e)
    {
        try
        {
            await DisposeAsync();
        }
        catch
        {
            // log if needed
        }
    }
}

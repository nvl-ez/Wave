using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.ServerManager;

namespace Wave.Ui.Pages.ExecutionContent.ViewModels;

public partial class ExecutionViewModel : ObservableObject, IQueryAttributable
{
    private readonly IServerExecutorService serverExecutorService;

    /***************************
    * VARIABLES AND PROPERTIES *
    ****************************/
    // Concurrency Management
    private Task? readLoopTask;
    private CancellationTokenSource? readLoopCts;
    private readonly SemaphoreSlim serverLock = new(1, 1); //Prevents multiple clicks

    // Main Objects
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    public partial ServerQuery? Server { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanStopServer))]
    [NotifyPropertyChangedFor(nameof(CanSendCommand))]
    [NotifyPropertyChangedFor(nameof(ServerState))]
    public partial IServerSession? ServerSession { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();

    // Properties
    public string? Name => Server?.Name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSendCommand))]
    public partial string CommandMessage { get; set; } = "";


    //State
    public bool CanStopServer => ServerSession is not null && ServerSession.IsRunning;
    public bool CanSendCommand => !string.IsNullOrEmpty(CommandMessage) && ServerSession is not null && ServerSession.IsRunning;
    public string ServerState => CanStopServer ? "Running" : "Stopped";

    /***************
    * CONSTRUCTORS *
    ***************/
    public ExecutionViewModel(IServerExecutorService serverExecutorService)
    {
        this.serverExecutorService = serverExecutorService;
    }

    /**********
    * METHODS *
    **********/
    //Comands
    [RelayCommand]
    public async Task LoadAsync()
    {
        if (Server is not null)
        {
            await StopReadLoopAsync();
            ServerSession = serverExecutorService.TryGetSession((Guid)Server.Id!);
            if (ServerSession is not null)
            {
                StartReadLoop(ServerSession);
                ServerSession.ServerDisposed += StopServer;
            }
        }
    }

    [RelayCommand]
    public async Task SendCommand()
    {
        if (ServerSession is not null)
            await ServerSession.SendCommandAsync(CommandMessage);
    }

    [RelayCommand]
    public async Task StopServer()
    {
        await DisposeServer();
    }

    [RelayCommand]
    public async Task StartServer()
    {
        if (Server is null)
            return;

        await serverLock.WaitAsync();
        try
        {
            // Start the new server/session first
            var newSession = await serverExecutorService.Start((Guid)Server.Id!);
            newSession.ServerDisposed += StopServer;

            // Stop only the old reader loop, not the old server process
            await StopReadLoopAsync();

            // Switch the current session reference
            ServerSession = newSession;

            // Start reading from the new session
            StartReadLoop(newSession);
        }
        finally
        {
            serverLock.Release();
        }
    }

    //Navigation
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {

        if (query.ContainsKey("server"))
        {
            Server = (ServerQuery)query["server"];
        }
    }

    //MISC
    private async Task StopReadLoopAsync()
    {
        var cts = readLoopCts;
        var task = readLoopTask;

        readLoopCts = null;
        readLoopTask = null;

        if (cts is not null)
        {
            cts.Cancel();
        }

        if (task is not null)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                // Normal when the loop is canceled
            }
        }

        cts?.Dispose();
    }

    private void StartReadLoop(IServerSession session)
    {
        var cts = new CancellationTokenSource();

        readLoopCts = cts;
        readLoopTask = ReadLoopAsync(session, cts.Token);
    }

    private async Task ReadLoopAsync(IServerSession session, CancellationToken ct)
    {
        Logs.Clear();
        try
        {
            await foreach (var line in session.GetOutputAsync(ct))
            {
                ct.ThrowIfCancellationRequested();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Logs.Add(line);

                    if (Logs.Count > 200)
                    {
                        Logs.RemoveAt(0); // remove oldest, not newest
                    }
                });
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Normal shutdown
        }
        catch (ObjectDisposedException) when (ct.IsCancellationRequested)
        {
            // Also acceptable during shutdown
        }
    }

    private async Task DisposeServer()
    {
        await serverLock.WaitAsync();
        try
        {
            var session = ServerSession;
            ServerSession = null;

            // Stop reading first
            await StopReadLoopAsync();

            // Then actually stop the server
            if (CanStopServer)
            {
                session!.ServerDisposed -= StopServer;
                await session!.DisposeAsync();

            }
        }
        finally
        {
            serverLock.Release();
        }
    }

    // Event Delegates
    private async void StopServer(object? sender, Guid id)
    {
        if (Server is not null && id == Server.Id)
        {
            await DisposeServer();
        }
    }
}

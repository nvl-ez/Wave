using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.ServerManager;
using Wave.Ui.Pages.ExecutionContent;
using Wave.Ui.Pages.ServerContent;

namespace Wave.Ui.Pages.ServersContent.ViewModels;

public partial class ServerCardViewModel : ObservableObject
{
    //SERVICES
    private readonly IServerExecutorService serverExecutorService;

    //STATES
    public string RunningState => IsRunning ? "Running" : "Stopped";

    [ObservableProperty]
    public partial ServerInfo ServerInfo { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    [NotifyPropertyChangedFor(nameof(RunningState))]
    public partial IServerSession? ServerSession { get; set; }

    public bool IsRunning => ServerSession is not null && ServerSession.IsRunning;

    public string? Name => ServerInfo?.Name;

    public ServerCardViewModel(ServerInfo serverInfo, IServerExecutorService serverExecutorService)
    {
        ServerInfo = serverInfo;
        this.serverExecutorService = serverExecutorService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        ServerSession = serverExecutorService.TryGetSession(ServerInfo.Id);
    }

    [RelayCommand]
    public async Task EditServerAsync()
    {
        var parameters = new ShellNavigationQueryParameters
        {
            { "server", ServerInfo.Id}
        };
        await Shell.Current.GoToAsync(nameof(ServerPage), parameters);
    }

    [RelayCommand]
    public async Task StartServerAsync()
    {
        if (!IsRunning)
        {
            ServerSession = await serverExecutorService.Start(ServerInfo.Id);
            ServerSession.ServerDisposed += StopServerAsync;
        }

        var parameters = new ShellNavigationQueryParameters
        {
            { "server", ServerInfo}
        };
        await Shell.Current.GoToAsync(nameof(ExecutionPage), parameters);
    }

    [RelayCommand]
    public async Task StopServerAsync()
    {
        await DisposeServer();
    }

    [RelayCommand]
    public async Task OpenServerAsync()
    {
        var parameters = new ShellNavigationQueryParameters
        {
            { "server", ServerInfo}
        };
        await Shell.Current.GoToAsync(nameof(ExecutionPage), parameters);
    }

    private async Task DisposeServer()
    {
        if (IsRunning)
        {
            await ServerSession!.DisposeAsync();
        }
        ServerSession!.ServerDisposed -= StopServerAsync;
        ServerSession = null;
    }

    private async void StopServerAsync(object? sender, Guid id)
    {
        await DisposeServer();
    }
}

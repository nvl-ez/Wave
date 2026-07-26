using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.ServerManager;
using Wave.Ui.Pages.ExecutionContent;
using Wave.Ui.Pages.ServerContent;

namespace Wave.Ui.Pages.ServersContent.ViewModels;

public partial class ServersViewModel : ObservableObject, IQueryAttributable
{
    private readonly IServerManagerService serverManagerService;
    private readonly IServerExecutorService serverExecutorService;

    public ObservableCollection<ServerCardViewModel> AllServers { get; private set; } = [];

    //Status
    [ObservableProperty]
    public partial string ServerListStatus { get; set; } = "Loading"; //Loading, Loaded

    public ServersViewModel(IServerManagerService serverManagerService, IServerExecutorService serverExecutorService)
    {
        this.serverManagerService = serverManagerService;
        this.serverExecutorService = serverExecutorService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        ServerListStatus = "Loading";

        AllServers.Clear();

        var serverCardViewModels = (await serverManagerService.GetAllServerQueriesAsync()).Select(s => new ServerCardViewModel(s, serverManagerService, serverExecutorService));
        foreach (var ServerCardViewModel in serverCardViewModels)
        {
            AllServers.Add(ServerCardViewModel);
        }

        ServerListStatus = "Loaded";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
    }

    [RelayCommand]
    private async Task NewServerAsync()
    {
        await Shell.Current.GoToAsync(nameof(ServerPage));
    }
}

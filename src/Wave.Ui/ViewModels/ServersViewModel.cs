using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;
using Wave.Infrastructure.Out.Minecraft.Api;
using Wave.Ui.Pages;

namespace Wave.Ui.ViewModels;

public partial class ServersViewModel : IQueryAttributable
{
    private readonly IServerManagerService serverManagerService;

    public ObservableCollection<ServerInfo> AllServers { get; private set; }

    public ServersViewModel(IServerManagerService serverManagerService)
    {
        this.serverManagerService = serverManagerService;
        AllServers = new ObservableCollection<ServerInfo>(serverManagerService.GetAll());
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("deleted"))
        {
            Guid deletedId = (Guid)query["deleted"];
            var existing = AllServers.FirstOrDefault(s => s.Id == deletedId);
            if (existing is not null)
                AllServers.Remove(existing);
        }
        else if (query.ContainsKey("saved"))
        {
            Guid serverId = (Guid)query["saved"];
            AllServers.Remove(AllServers.First(s => s.Id == serverId));
            AllServers.Add(await serverManagerService.GetAsync(serverId));
        }
        else if (query.ContainsKey("created"))
        {
            Guid serverId = (Guid)query["created"];
            AllServers.Add(await serverManagerService.GetAsync(serverId));
        }
    }

    [RelayCommand]
    private async Task NewServerAsync()
    {
        await Shell.Current.GoToAsync(nameof(ServerPage));
    }

    [RelayCommand]
    public async Task EditServerAsync(ServerInfo info)
    {
        var parameters = new ShellNavigationQueryParameters
        {
            { "server", info!.Id}
        };
        await Shell.Current.GoToAsync(nameof(ServerPage), parameters);
    }

    [RelayCommand]
    public async Task StartServerAsync(ServerInfo info)
    {

    }
}

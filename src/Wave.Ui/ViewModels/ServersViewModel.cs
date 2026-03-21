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
    private IServerCatalogService serverCatalogService;

    public ObservableCollection<Server> AllServers { get; private set; }

    public ServersViewModel(IServerCatalogService serverCatalogService)
    {
        this.serverCatalogService = serverCatalogService;
        AllServers = new ObservableCollection<Server>(serverCatalogService.GetServers());
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
            AllServers.Add(await serverCatalogService.GetServerAsync(serverId));
        }
    }

    [RelayCommand]
    private async Task NewServerAsync()
    {
        await Shell.Current.GoToAsync(nameof(ServerPage));
    }

    [RelayCommand]
    public async Task EditServerAsync(Server server)
    {
        var parameters = new ShellNavigationQueryParameters
        {
            { "server", server!.Id}
        };
        await Shell.Current.GoToAsync(nameof(ServerPage), parameters);
    }

    [RelayCommand]
    public async Task StartServerAsync(Server server)
    {

    }

    [RelayCommand]
    public async Task DeleteServerAsync(Server server)
    {
        await serverCatalogService.DeleteAsync(server, CancellationToken.None);
        AllServers.Remove(AllServers.First(s => s.Id == server.Id));
    }
}

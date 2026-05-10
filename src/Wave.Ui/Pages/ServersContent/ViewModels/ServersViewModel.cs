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

    public ObservableCollection<ServerCardViewModel> AllServers { get; private set; }

    public ServersViewModel(IServerManagerService serverManagerService, IServerExecutorService serverExecutorService)
    {
        this.serverManagerService = serverManagerService;
        this.serverExecutorService = serverExecutorService;

        AllServers = new ObservableCollection<ServerCardViewModel>(serverManagerService.GetAll().Select(s => new ServerCardViewModel(s, serverExecutorService)));
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("deleted"))
        {
            Guid deletedId = (Guid)query["deleted"];
            var existing = AllServers.FirstOrDefault(s => s.ServerInfo.Id == deletedId);
            if (existing is not null)
                AllServers.Remove(existing);
        }
        else if (query.ContainsKey("saved"))
        {
            Guid serverId = (Guid)query["saved"];
            AllServers.Remove(AllServers.First(s => s.ServerInfo.Id == serverId));

            ServerInfo info = await serverManagerService.GetServerInfoAsync(serverId);
            AllServers.Add(new ServerCardViewModel(info, serverExecutorService));
        }
        else if (query.ContainsKey("created"))
        {
            Guid serverId = (Guid)query["created"];

            ServerInfo info = await serverManagerService.GetServerInfoAsync(serverId);
            AllServers.Add(new ServerCardViewModel(info, serverExecutorService));
        }
    }

    [RelayCommand]
    private async Task NewServerAsync()
    {
        await Shell.Current.GoToAsync(nameof(ServerPage));
    }
}

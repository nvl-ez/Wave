using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.Out.ServerManager;
using Wave.Application.Services;
using Wave.Infrastructure.Out.Minecraft.Api;
using Wave.Ui.Pages;

namespace Wave.Ui.ViewModels;

public partial class ServersViewModel : IQueryAttributable
{
    private IServerRepository serverRepository;

    public ObservableCollection<ServerViewModel> AllServers { get; }

    //TODO Se deberia de acceder al repo por un servicio
    public ServersViewModel(IServerRepository serverRepository)
    {
        this.serverRepository = serverRepository;
        AllServers = new ObservableCollection<ServerViewModel>(serverRepository.GetServers().Select(s => new ServerViewModel(s, serverRepository, new MinecraftVersionCatalogService(new MinecraftVersionCatalog())))); //TODO Mover al AppComposition
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {

    }

    [RelayCommand]
    private async Task NewServerAsync()
    {
        await Shell.Current.GoToAsync(nameof(ServerPage));
    }

    [RelayCommand]
    public async Task EditServerAsync(ServerViewModel serverViewModel)
    {
        Dictionary<string, object> parameters = new()
        {
            { "server", serverViewModel.Server!}
        };
        await Shell.Current.GoToAsync(nameof(ServerPage), parameters);
    }

    [RelayCommand]
    public async Task DeleteServerAsync(ServerViewModel serverViewModel)
    {
        await serverRepository.DeleteAsync(serverViewModel.Server!, CancellationToken.None);
        AllServers.Remove(serverViewModel);
    }
}

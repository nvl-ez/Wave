using System;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Application.Out.Minecraft.Api;
using Wave.Application.Out.ServerManager;
using Wave.Domain;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;

namespace Wave.Ui.ViewModels;

public partial class ServerViewModel : ObservableObject, IQueryAttributable
{
    /***************************
    * VARIABLES AND PROPERTIES *
    ***************************/
    //Pseudo Navigation
    [ObservableProperty]
    public partial string CurrentTab { get; set; } = "General";


    //Statuses
    [ObservableProperty]
    public partial string MinecraftVersionsStatus { get; set; } = "Loading"; //Loading, Done, Error

    //Server
    public Server? Server;
    private IServerRepository serverRepository;
    IMinecraftVersionCatalogService minecraftVersionCatalog;

    public string TitleName => Server is null ? "New Server" : Name;
    public string Name
    {
        get => Server is null ? "" : Server.Name;
        set
        {
            if (Server is not null && value != Server.Name)
            {
                Server.Name = value;
                OnPropertyChanged();
            }
        }
    }
    public string Motd
    {
        get => Server is null ? "" : Server.Motd;
        set
        {
            if (Server is not null && value != Server.Motd)
            {
                Server.Motd = value;
                OnPropertyChanged();
            }
        }
    }
    public DateTime? CreationDate => Server is null ? null : Server.CreationDate;
    //Versions
    [ObservableProperty]
    public partial List<MinecraftVersion> MinecraftVersions { get; set; } = new List<MinecraftVersion>();
    [ObservableProperty]
    public partial MinecraftVersion? SelectedMinecraftVersion { get; set; } = null;
    [ObservableProperty]
    public partial bool IncludeSnapshots { get; set; } = false;
    //Gamemode
    public IList<KeyValuePair<string, string>> Gamemodes = new List<KeyValuePair<string, string>>()
    {
        new(){}
    };

    /***************
    * CONSTRUCTORS *
    ***************/
    //TODO: Se deberia de acceder al repo por un servicio
    public ServerViewModel(IServerRepository serverRepository, IMinecraftVersionCatalogService minecraftVersionCatalog)
    {
        this.Server = null;
        this.serverRepository = serverRepository;
        this.minecraftVersionCatalog = minecraftVersionCatalog;
    }

    //TODO: Se deberia de acceder al repo por un servicio
    public ServerViewModel(Server server, IServerRepository serverRepository, IMinecraftVersionCatalogService minecraftVersionCatalog)
    {
        this.Server = server;
        this.serverRepository = serverRepository;
        this.minecraftVersionCatalog = minecraftVersionCatalog;
    }


    /**********
    * METHODS *
    **********/
    //Pseudo navigation
    [RelayCommand]
    public void ShowGeneralView()
    {
        CurrentTab = "General";
    }
    [RelayCommand]
    public void ShowModsView()
    {
        CurrentTab = "Mods";
    }
    [RelayCommand]
    public void ShowPropertiesView()
    {
        CurrentTab = "Properties";
    }

    //General
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        CurrentTab = "General";

        if (query.ContainsKey("server"))
        {
            Server = (Server)query["server"];
        }

        RefreshProperties();
    }
    private void RefreshProperties()
    {
        //Añadir para cada property
        OnPropertyChanged(nameof(CurrentTab));

        OnPropertyChanged(nameof(MinecraftVersions));

        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Motd));
    }

    //API Petitions
    [RelayCommand]
    public async Task Load()
    {
        await RequestMinecraftVersionsAsync();
        RefreshProperties();
    }

    [RelayCommand]
    private async Task RequestMinecraftVersionsAsync()
    {
        MinecraftVersionsStatus = "Loading";
        MinecraftVersions = (List<MinecraftVersion>)await minecraftVersionCatalog.GetMinecraftVersionsAsync(IncludeSnapshots, CancellationToken.None);
        if (MinecraftVersions is not null && MinecraftVersions.Count > 0) MinecraftVersionsStatus = "Done";
        else MinecraftVersionsStatus = "Error";
    }

    //Server
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Server is null) return;
        await serverRepository.SaveAsync(Server, CancellationToken.None);
    }
    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (Server is null) return;
        await serverRepository.DeleteAsync(Server, CancellationToken.None);
    }
    [RelayCommand]
    public async Task StartAsync()
    {

    }


    public enum Tab
    {
        General,
        Mods,
        Properties
    }
}

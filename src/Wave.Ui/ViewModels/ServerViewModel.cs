using System;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain;
using Wave.Domain.Minecraft;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;
using Wave.Ui.Pages;

namespace Wave.Ui.ViewModels;

public partial class ServerViewModel : ObservableObject, IQueryAttributable
{
    private readonly IServerCatalogService serverCatalogService;
    private readonly IMinecraftCatalogService minecraftCatalogService;

    /***************************
    * VARIABLES AND PROPERTIES *
    ************P***************/
    //Pseudo Navigation
    [ObservableProperty]
    public partial string CurrentTab { get; set; } = "General";


    //Statuses
    [ObservableProperty]
    public partial string MinecraftVersionsStatus { get; set; } = "Loading"; //Loading, Done, Error

    //Server
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleName))]
    [NotifyPropertyChangedFor(nameof(Name))]
    [NotifyPropertyChangedFor(nameof(Motd))]
    private partial Server? Server { get; set; }

    public string TitleName => Server is null ? "New Server" : Name;
    public string Name
    {
        get => Server is null ? "" : Server.Name;
        set
        {
            if (Server is not null && value != Server.Name)
            {
                Server.Name = value;
            }
        }
    }
    public string Motd
    {
        get => Server is null || !Server.Properties.ContainsKey("motd") ? "" : Server.Properties["motd"];
        set
        {
            if (Server is not null && Server.Properties.ContainsKey("motd") && value != Server.Properties["motd"])
            {
                Server.Properties["motd"] = value;
            }
        }
    }
    public MinecraftVersion? MinecraftVersion
    {
        get => Server?.MinecraftVersion;
        set
        {
            if (value is null) return;
            if (Server is not null)
            {
                Server.MinecraftVersion = value;
            }
        }
    }
    public DateTime? CreationDate => Server is null ? null : Server.CreationDate;
    //Versions
    [ObservableProperty]
    public partial List<MinecraftVersion> MinecraftVersions { get; set; } = new List<MinecraftVersion>();
    [ObservableProperty]
    public partial bool IncludeSnapshots { get; set; } = false;
    [ObservableProperty]
    public partial int MinecraftVersionsIndex { get; set; } = -1;

    /***************
    * CONSTRUCTORS *
    ***************/
    //TODO: Se deberia de acceder al repo por un servicio
    public ServerViewModel(IServerCatalogService serverCatalogService, IMinecraftCatalogService minecraftCatalogService)
    {
        this.Server = null;
        this.serverCatalogService = serverCatalogService;
        this.minecraftCatalogService = minecraftCatalogService;
    }

    //TODO: Se deberia de acceder al repo por un servicio
    public ServerViewModel(Server server, IServerCatalogService serverCatalogService, IMinecraftCatalogService minecraftCatalogService)
    {
        this.Server = server;
        this.serverCatalogService = serverCatalogService;
        this.minecraftCatalogService = minecraftCatalogService;
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
    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        CurrentTab = "General";

        if (query.ContainsKey("server"))
        {
            Guid serverId = (Guid)query["server"];
            Server = await serverCatalogService.GetServerAsync(serverId);
        }
    }


    //API Petitions
    [RelayCommand]
    public async Task LoadAsync()
    {
        await RequestMinecraftVersionsAsync();
    }

    [RelayCommand]
    private async Task RequestMinecraftVersionsAsync()
    {
        MinecraftVersionsStatus = "Loading";
        MinecraftVersionQuery query = new MinecraftVersionQuery() { IncludeSnapshots = IncludeSnapshots };
        MinecraftVersions = (List<MinecraftVersion>)await minecraftCatalogService.GetMinecraftVersionsAsync(query);
        if (MinecraftVersions is not null && MinecraftVersions.Count > 0)
        {
            MinecraftVersionsStatus = "Done";
            if (MinecraftVersion is not null)
            {
                MinecraftVersionsIndex = MinecraftVersions.FindIndex(v => v.Version == MinecraftVersion.Version);
            }
        }
        else MinecraftVersionsStatus = "Error";
    }

    //Server
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Server is null) return;
        await serverCatalogService.SaveAsync(Server);
        var parameters = new ShellNavigationQueryParameters
        {
            { "saved", Server!.Id}
        };
        await Shell.Current.GoToAsync("..", parameters);
    }
    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (Server is null) return;

        await serverCatalogService.DeleteAsync(Server.Id);
        var parameters = new ShellNavigationQueryParameters
        {
            { "deleted", Server!.Id}
        };
        await Shell.Current.GoToAsync("..", parameters);
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

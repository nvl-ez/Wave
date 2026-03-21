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
    private readonly IServerHandlerService serverHandlerService;

    /***************************
    * VARIABLES AND PROPERTIES *
    ************P***************/
    //Pseudo Navigation
    [ObservableProperty]
    public partial string CurrentTab { get; set; } = "General";


    //Statuses
    [ObservableProperty]
    public partial string MinecraftVersionsStatus { get; set; } = "Loading"; //Loading, Done, Error
    [ObservableProperty]
    public partial string ServerPropertiesStatus { get; set; } = "Loading";
    public string ServerStatus => Server.IsReady ? "Ready" : "New"; //New, Ready

    //Server
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleName))]
    [NotifyPropertyChangedFor(nameof(Name))]
    [NotifyPropertyChangedFor(nameof(Motd))]
    [NotifyPropertyChangedFor(nameof(MinecraftVersion))]
    [NotifyPropertyChangedFor(nameof(MinecraftVersionIndex))]
    [NotifyPropertyChangedFor(nameof(GamemodeValue))]
    [NotifyPropertyChangedFor(nameof(DifficultyValue))]
    [NotifyPropertyChangedFor(nameof(MotdValue))]
    [NotifyPropertyChangedFor(nameof(ServerIpValue))]
    [NotifyPropertyChangedFor(nameof(MaxPlayersValue))]
    [NotifyPropertyChangedFor(nameof(ServerStatus))]
    private partial Server Server { get; set; } = new Server();

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
    public DateTime? CreationDate => Server is null ? null : Server.CreationDate;

    //Versions
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
    [ObservableProperty]
    public partial List<MinecraftVersion> MinecraftVersions { get; set; } = new List<MinecraftVersion>();
    [ObservableProperty]
    public partial bool IncludeSnapshots { get; set; } = false;
    public int? MinecraftVersionIndex => MinecraftVersions?.FindIndex(mv => mv.Version == MinecraftVersion?.Version);

    //Gamemode
    public string? GamemodeValue
    {
        get { return Server?.Properties["gamemode"]; }
        set { Server?.Properties["gamemode"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? GamemodeServerProperty { get; set; } = null;
    //Difficulty
    public string? DifficultyValue
    {
        get { return Server?.Properties["difficulty"]; }
        set { Server?.Properties["difficulty"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? DifficultyServerProperty { get; set; } = null;
    //Motd
    public string? MotdValue
    {
        get { return Server?.Properties["motd"]; }
        set { Server?.Properties["motd"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? MotdServerProperty { get; set; } = null;
    //Ip
    public string? ServerIpValue
    {
        get { return Server?.Properties["server-ip"]; }
        set { Server?.Properties["server-ip"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? ServerIpServerProperty { get; set; } = null;
    //Max Players
    public string? MaxPlayersValue
    {
        get { return Server?.Properties["max-players"]; }
        set { Server?.Properties["max-players"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? MaxPlayersServerProperty { get; set; } = null;

    /***************
    * CONSTRUCTORS *
    ***************/
    public ServerViewModel(IServerCatalogService serverCatalogService, IMinecraftCatalogService minecraftCatalogService, IServerHandlerService serverHandlerService)
    {
        this.serverCatalogService = serverCatalogService;
        this.minecraftCatalogService = minecraftCatalogService;
        this.serverHandlerService = serverHandlerService;
    }

    public ServerViewModel(Server server, IServerCatalogService serverCatalogService, IMinecraftCatalogService minecraftCatalogService, IServerHandlerService serverHandlerService)
    {
        this.Server = server;
        this.serverCatalogService = serverCatalogService;
        this.minecraftCatalogService = minecraftCatalogService;
        this.serverHandlerService = serverHandlerService;
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
        else
        {
            Server = new Server();
        }
    }


    //API Petitions
    [RelayCommand]
    public async Task LoadAsync()
    {
        await RequestMinecraftVersionsAsync();
        RequestServerPropertiesAsync();
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
        }
        else MinecraftVersionsStatus = "Error";
    }

    private void RequestServerPropertiesAsync()
    {
        ServerPropertiesStatus = "Loading";

        GamemodeServerProperty = minecraftCatalogService.GetServerPropertyDefinition("gamemode");
        DifficultyServerProperty = minecraftCatalogService.GetServerPropertyDefinition("difficulty");


        ServerPropertiesStatus = "Done";

    }

    //Server
    [RelayCommand]
    public async Task SaveAsync()
    {

    }

    [RelayCommand]
    public async Task CreateAsync()
    {
        if (Server is null) return;
        await serverHandlerService.CreateAsync(Server);
        var parameters = new ShellNavigationQueryParameters
        {
            { "created", Server!.Id}
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

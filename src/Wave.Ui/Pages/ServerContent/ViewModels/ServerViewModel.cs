using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;

//TODO: Server Eula automatization

namespace Wave.Ui.Pages.ServerContent.ViewModels;

public partial class ServerViewModel : ObservableObject, IQueryAttributable
{
    private readonly IMinecraftCatalogService minecraftCatalogService;
    private readonly IServerManagerService serverHandlerService;

    /***************************
    * VARIABLES AND PROPERTIES *
    ************P***************/
    //Pseudo Navigation
    [ObservableProperty]
    public partial string CurrentTab { get; set; } = "General";


    //States
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
    private ServerInfo Info => Server.Info;
    private ServerDetails Details => Server.Details;

    public string TitleName => Server is null ? "New Server" : Name;
    public string Name
    {
        get => Server is null ? "" : Info.Name;
        set
        {
            if (Server is not null && value != Info.Name)
            {
                Info.Name = value;
            }
        }
    }
    public string Motd
    {
        get => Details.Properties.ContainsKey("motd") ? Details.Properties["motd"] : "";
        set
        {
            if (Server is not null && Details.Properties.ContainsKey("motd") && value != Details.Properties["motd"])
            {
                Details.Properties["motd"] = value;
            }
        }
    }
    public DateTime? CreationDate => Server is null ? null : Info.CreationDate;

    //Versions
    public MinecraftVersion? MinecraftVersion
    {
        get => Details.MinecraftVersion;
        set
        {
            if (value is null) return;
            if (Server is not null)
            {
                Details.MinecraftVersion = value;
            }
        }
    }
    [ObservableProperty]
    public partial List<MinecraftVersion> MinecraftVersions { get; set; } = new List<MinecraftVersion>();
    [ObservableProperty]
    public partial bool IncludeSnapshots { get; set; } = false;
    public int? MinecraftVersionIndex => MinecraftVersions?.FindIndex(mv => mv.Version == MinecraftVersion?.Version);

    //Gamemode
    public string GamemodeValue
    {
        get { return Details.Properties["gamemode"]; }
        set { Details.Properties["gamemode"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? GamemodeServerProperty { get; set; } = null;
    //Difficulty
    public string DifficultyValue
    {
        get { return Details.Properties["difficulty"]; }
        set { Details.Properties["difficulty"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? DifficultyServerProperty { get; set; } = null;
    //Motd
    public string MotdValue
    {
        get { return Details.Properties["motd"]; }
        set { Details.Properties["motd"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? MotdServerProperty { get; set; } = null;
    //Ip
    public string ServerIpValue
    {
        get { return Details.Properties["server-ip"]; }
        set { Details.Properties["server-ip"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? ServerIpServerProperty { get; set; } = null;
    //Max Players
    public string MaxPlayersValue
    {
        get { return Details.Properties["max-players"]; }
        set { Details.Properties["max-players"] = value; }
    }
    [ObservableProperty]
    public partial ServerPropertyDefinition? MaxPlayersServerProperty { get; set; } = null;

    /***************
    * CONSTRUCTORS *
    ***************/
    public ServerViewModel(IMinecraftCatalogService minecraftCatalogService, IServerManagerService serverHandlerService)
    {
        this.minecraftCatalogService = minecraftCatalogService;
        this.serverHandlerService = serverHandlerService;
    }

    public ServerViewModel(Server server, IMinecraftCatalogService minecraftCatalogService, IServerManagerService serverHandlerService)
    {
        this.Server = server;
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
            Server = await serverHandlerService.LoadAsync(serverId); //TODO: Cambiar para cargar servers pasados por guid
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

        await serverHandlerService.DeleteAsync(Server);
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

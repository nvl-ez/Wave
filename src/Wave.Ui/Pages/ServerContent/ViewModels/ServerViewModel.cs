using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Properties;

//TODO: Server Eula automatization

namespace Wave.Ui.Pages.ServerContent.ViewModels;

public partial class ServerViewModel : ObservableObject, IQueryAttributable
{
    private readonly IMinecraftCatalogService minecraftCatalogService;
    private readonly IServerManagerService serverHandlerService;

    /***************************
    * VARIABLES AND PROPERTIES *
    ****************************/
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
    [NotifyPropertyChangedFor(nameof(MinecraftVersionInfo))]
    [NotifyPropertyChangedFor(nameof(MinecraftVersionIndex))]
    [NotifyPropertyChangedFor(nameof(GamemodeValue))]
    [NotifyPropertyChangedFor(nameof(DifficultyValue))]
    [NotifyPropertyChangedFor(nameof(MotdValue))]
    [NotifyPropertyChangedFor(nameof(ServerIpValue))]
    [NotifyPropertyChangedFor(nameof(MaxPlayersValue))]
    [NotifyPropertyChangedFor(nameof(ServerStatus))]
    [NotifyPropertyChangedFor(nameof(EulaIndex))]
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
    public MinecraftVersionInfo? MinecraftVersionInfo
    {
        get => Details.MinecraftVersion;
        set
        {
            if (value is null) return;

            Details.MinecraftVersion = value;
        }
    }
    [ObservableProperty]
    public partial List<MinecraftVersionInfo> MinecraftVersionsInfos { get; set; } = new List<MinecraftVersionInfo>();
    [ObservableProperty]
    public partial bool IncludeSnapshots { get; set; } = false;
    public int? MinecraftVersionIndex => MinecraftVersionsInfos?.FindIndex(mv => mv.MinecraftVersion == MinecraftVersionInfo?.MinecraftVersion);

    //Gamemode
    public string GamemodeValue
    {
        get { return Details.Properties["gamemode"]; }
        set { Details.Properties["gamemode"] = value; }
    }
    [ObservableProperty]
    public partial PropertyDefinition? GamemodeServerProperty { get; set; } = null;
    //Difficulty
    public string DifficultyValue
    {
        get { return Details.Properties["difficulty"]; }
        set { Details.Properties["difficulty"] = value; }
    }
    [ObservableProperty]
    public partial PropertyDefinition? DifficultyServerProperty { get; set; } = null;
    //Motd
    public string MotdValue
    {
        get { return Details.Properties["motd"]; }
        set { Details.Properties["motd"] = value; }
    }
    [ObservableProperty]
    public partial PropertyDefinition? MotdServerProperty { get; set; } = null;
    //Ip
    public string ServerIpValue
    {
        get { return Details.Properties["server-ip"]; }
        set { Details.Properties["server-ip"] = value; }
    }
    [ObservableProperty]
    public partial PropertyDefinition? ServerIpServerProperty { get; set; } = null;
    //Max Players
    public string MaxPlayersValue
    {
        get { return Details.Properties["max-players"]; }
        set { Details.Properties["max-players"] = value; }
    }
    [ObservableProperty]
    public partial PropertyDefinition? MaxPlayersServerProperty { get; set; } = null;
    //Eula
    public List<KeyValuePair<bool, string>> EulaOptions { get; set; } = [new KeyValuePair<bool, string>(true, "Agree"), new KeyValuePair<bool, string>(false, "Disagree")];
    public int EulaIndex
    {
        get
        {
            return EulaOptions.FindIndex(e => e.Key == Server.Details.Eula);
        }
        set
        {
            if (value >= 0 && value < EulaOptions.Count)
                Server.Details.Eula = EulaOptions[value].Key;
        }
    }

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
            Server = await serverHandlerService.GetServerAsync(serverId);
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
        MinecraftVersionsInfos = (List<MinecraftVersionInfo>)await minecraftCatalogService.GetMinecraftVersionsAsync(query);
        if (MinecraftVersionsInfos is not null && MinecraftVersionsInfos.Count > 0)
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
        await serverHandlerService.EditServerAsync(Server);
        var parameters = new ShellNavigationQueryParameters
        {
            { "saved", Server!.Id}
        };
        await Shell.Current.GoToAsync("..", parameters);
    }

    [RelayCommand]
    public async Task CreateAsync()
    {
        if (Server is null) return;
        await serverHandlerService.CreateServerAsync(Server);
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

        await serverHandlerService.DeleteServerAsync(Server);
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

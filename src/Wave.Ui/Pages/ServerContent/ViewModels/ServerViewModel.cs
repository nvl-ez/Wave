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
    public partial string CurrentTab { get; set; } = "General"; //General, Mods, Properties


    //States
    [ObservableProperty]
    public partial string MinecraftVersionsStatus { get; set; } = "Loading"; //Loading, Done, Error
    [ObservableProperty]
    public partial string ServerPropertiesStatus { get; set; } = "Loading"; //Loading, Done, Error

    //Server
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleName))]
    [NotifyPropertyChangedFor(nameof(MinecraftVersionIndex))]
    public partial ServerQuery Server { get; set; } = new();

    public string TitleName => Server.Id is null ? "New Server" : Server.Name!;
    public DateTime? CreationDate => Server is null ? null : Server.CreationDate;

    //Versions
    [ObservableProperty]
    public partial List<MinecraftVersionInfo> MinecraftVersionsInfos { get; set; } = new List<MinecraftVersionInfo>();
    [ObservableProperty]
    public partial bool IncludeSnapshots { get; set; } = false;
    public int? MinecraftVersionIndex => MinecraftVersionsInfos?.FindIndex(mv => mv.MinecraftVersion == Server.MinecraftVersionBase?.MinecraftVersion);

    //Gamemode
    [ObservableProperty]
    public partial PropertyDefinition? GamemodeServerProperty { get; set; } = null;
    //Difficulty
    [ObservableProperty]
    public partial PropertyDefinition? DifficultyServerProperty { get; set; } = null;
    //Motd
    [ObservableProperty]
    public partial PropertyDefinition? MotdServerProperty { get; set; } = null;
    //Ip
    [ObservableProperty]
    public partial PropertyDefinition? ServerIpServerProperty { get; set; } = null;
    //Max Players
    [ObservableProperty]
    public partial PropertyDefinition? MaxPlayersServerProperty { get; set; } = null;

    //Eula
    public List<KeyValuePair<bool, string>> EulaOptions { get; set; } = [new KeyValuePair<bool, string>(true, "Agree"), new KeyValuePair<bool, string>(false, "Disagree")]; //TODO: Arreglar eula


    /***************
    * CONSTRUCTORS *
    ***************/
    public ServerViewModel(IMinecraftCatalogService minecraftCatalogService, IServerManagerService serverHandlerService)
    {
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
            Server = await serverHandlerService.GetServerQueryAsync(serverId);
        }
        else
        {
            Server = new ServerQuery();
        }
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RequestMinecraftVersionsAsync();
        RequestServerPropertiesAsync();
    }

    //API Petitions
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

    private async void RequestServerPropertiesAsync()
    {
        ServerPropertiesStatus = "Loading";

        GamemodeServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("gamemode");
        DifficultyServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("difficulty");
        MotdServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("motd");
        ServerIpServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("server-ip");
        MaxPlayersServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("max-players");

        ServerPropertiesStatus = "Done";
    }

    //Server
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Server.Id is not null)
            await serverHandlerService.EditServerAsync(Server);
        else
            await serverHandlerService.CreateServerAsync(Server);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (Server?.Id is null) return;

        await serverHandlerService.DeleteServerAsync((Guid)Server.Id);
        var parameters = new ShellNavigationQueryParameters
        {
            { "deleted", Server.Id}
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

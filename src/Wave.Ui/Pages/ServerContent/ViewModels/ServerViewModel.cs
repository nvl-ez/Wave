using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.Minecraft;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;
using Wave.Domain.ServerManager.Properties;
using Wave.Domain.Utils;
using Wave.Ui.Pages.ServerContent.Classes;
using Wave.Ui.Pages.ServerContent.Views;
using Wave.Ui.Utils;

//TODO: Server Eula automatization

namespace Wave.Ui.Pages.ServerContent.ViewModels;

public partial class ServerViewModel : ObservableObject, IQueryAttributable
{
    private readonly IMinecraftCatalogService minecraftCatalogService;
    private readonly IServerManagerService serverHandlerService;
    private readonly IModloaderCatalogService modloaderCatalogService;
    private readonly IModCatalogService modCatalogService;

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
    [ObservableProperty]
    public partial string ModloaderInfoStatus { get; set; } = "Loading"; // Loading, Done, Error
    [ObservableProperty]
    public partial bool ModsSectionIsVisible { get; set; } = false;
    [ObservableProperty]
    public partial bool ModloaderInfoIsVisible { get; set; } = false;
    private bool IsModalOpen { get; set; } = false;

    //Server
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleName))]
    public partial ServerQuery Server { get; set; } = new();

    public string TitleName => Server.Id is null ? "New Server" : Server.Name!;
    public DateTime? CreationDate => Server is null ? null : Server.CreationDate;

    //Versions
    [ObservableProperty]
    public partial List<MinecraftVersionInfo> MinecraftVersionsInfos { get; set; } = new List<MinecraftVersionInfo>();
    [ObservableProperty]
    public partial bool IncludeSnapshots { get; set; } = false;

    //Properties
    [ObservableProperty]
    public partial PropertyDefinition? GamemodeServerProperty { get; set; } = null;
    [ObservableProperty]
    public partial PropertyDefinition? DifficultyServerProperty { get; set; } = null;
    [ObservableProperty]
    public partial PropertyDefinition? MotdServerProperty { get; set; } = null;
    [ObservableProperty]
    public partial PropertyDefinition? ServerIpServerProperty { get; set; } = null;
    [ObservableProperty]
    public partial PropertyDefinition? MaxPlayersServerProperty { get; set; } = null;

    //Eula
    public List<KeyValuePair<bool, string>> EulaOptions { get; set; } = [new KeyValuePair<bool, string>(true, "Agree"), new KeyValuePair<bool, string>(false, "Disagree")]; //TODO: Arreglar eula

    //Modloaders
    public ObservableCollection<KeyValuePair<ModloaderType?, string>> ModloaderTypes { get; set; } = [];
    [ObservableProperty]
    public partial ModloaderTypeQuery ModloaderTypeQuery { get; set; } = new();
    [ObservableProperty]
    public partial ModloaderInfo? SelectedModloaderInfo { get; set; } = null;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledModloaderIsVisible))]
    public partial ModloaderBase? InstalledModloader { get; set; } = null;
    public bool InstalledModloaderIsVisible => InstalledModloader is not null;
    public ObservableCollection<ModloaderInfo> ModloaderInfos { get; set; } = [];

    //Mods
    public ObservableCollection<ModCardViewModel> Mods { get; set; } = new();


    /***************
    * CONSTRUCTORS *
    ***************/
    public ServerViewModel(IMinecraftCatalogService minecraftCatalogService, IServerManagerService serverHandlerService, IModloaderCatalogService modloaderCatalogService, IModCatalogService modCatalogService)
    {
        this.minecraftCatalogService = minecraftCatalogService;
        this.serverHandlerService = serverHandlerService;
        this.modloaderCatalogService = modloaderCatalogService;
        this.modCatalogService = modCatalogService;
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
        if (IsModalOpen) return;

        CurrentTab = "General";
        ServerQuery server;

        if (query.ContainsKey("server"))
            server = await serverHandlerService.GetServerQueryAsync((Guid)query["server"]);
        else
            server = new ServerQuery();
        IsModalOpen = false;


        //Obterner versiones de MC
        IncludeSnapshots = server.MinecraftVersionBase?.MinecraftVersionType == MinecraftVersionType.Snapshot;
        await RequestMinecraftVersionsAsync();

        //Obtener server.property definitions
        await RequestServerPropertiesAsync();

        //Obtener modloaders disponibles y setear variables
        await RequestModloadersAsync();
        ModloaderInfoIsVisible = false;
        ModloaderInfos.Clear();
        SelectedModloaderInfo = null;
        ModloaderTypeQuery = new();
        InstalledModloader = server.Modloader;

        //Añadir mods
        Mods.Clear();
        foreach (var mod in server.Mods)
        {
            Mods.Add(new ModCardViewModel(mod, RemoveModFileCommand));
        }

        //Update al properties
        Server = new ServerQuery();
        Server = server;
    }

    partial void OnSelectedModloaderInfoChanged(ModloaderInfo? value)
    {
        if (value is null) return;

        Server.Modloader = value;
        InstalledModloader = value;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {

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

    private async Task RequestServerPropertiesAsync()
    {
        ServerPropertiesStatus = "Loading";

        GamemodeServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("gamemode");
        DifficultyServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("difficulty");
        MotdServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("motd");
        ServerIpServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("server-ip");
        MaxPlayersServerProperty = await minecraftCatalogService.GetServerPropertyDefinitionAsync("max-players");

        ServerPropertiesStatus = "Done";
    }

    private async Task RequestModloadersAsync()
    {
        ModloaderTypes.Clear();
        foreach (var modloader in await modloaderCatalogService.GetModloaderTypesAsync())
        {
            ModloaderTypes.Add(new KeyValuePair<ModloaderType?, string>(modloader.Key, modloader.Value));
        }
    }

    [RelayCommand]
    private async Task RequestModloaderVersionsAsync()
    {
        ModloaderInfoIsVisible = false;
        ModloaderInfos.Clear();
        SelectedModloaderInfo = null;

        string? minecraftVersion = Server.MinecraftVersionBase?.MinecraftVersion;
        if (minecraftVersion is null) return;
        if (ModloaderTypeQuery.ModloaderType is null) return;

        var modloaderInfos = await modloaderCatalogService.GetModloaderVersionsAsync(
                (ModloaderType)ModloaderTypeQuery.ModloaderType,
                minecraftVersion
            );

        foreach (var modloaderInfo in modloaderInfos)
        {
            ModloaderInfos.Add(modloaderInfo);
        }
        ModloaderInfoIsVisible = true;
    }

    [RelayCommand]
    private void RemoveModloader()
    {
        Server.Modloader = null;
        InstalledModloader = null;
    }

    [RelayCommand]
    private void RemoveModFile(ModFile modFile)
    {
        if (modFile is null) return;

        Server.Mods = Server.Mods.Where(mod => !mod.Equals(modFile));

        var modCard = Mods.FirstOrDefault(mod => mod.ModFile?.Equals(modFile) == true);
        if (modCard is not null) Mods.Remove(modCard);
    }

    //Server
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (Server.Id is not null)
        {
            ServerChanges? changes = await serverHandlerService.EditServerAsync(Server);
            if (changes is not null)
            {
                IsModalOpen = true;
                try
                {
                    var popup = new ChangesPopup(changes);
                    await Shell.Current.ShowPopupAsync(popup);
                }
                finally
                {
                    IsModalOpen = false;
                }

                return;
            }
        }
        else await serverHandlerService.CreateServerAsync(Server);

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
    public async Task OpenModsPopupAsync()
    {
        IsModalOpen = true;
        try
        {
            var popup = new ModsPopup(Server, modCatalogService);
            await Shell.Current.ShowPopupAsync(popup);
        }
        finally
        {
            IsModalOpen = false;
        }

        Mods.Clear();

        foreach (var mod in Server.Mods)
        {
            Mods.Add(new ModCardViewModel(mod, RemoveModFileCommand));
        }
    }



    public enum Tab
    {
        General,
        Mods,
        Properties
    }
}

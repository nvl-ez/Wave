using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.Minecraft;
using Wave.Domain.Java;
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
    private readonly IJavaManagerService javaManagerService;

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
    private byte[]? pendingServerIcon;

    //Server
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleName))]
    [NotifyPropertyChangedFor(nameof(IsServerCreated))]
    public partial ServerQuery Server { get; set; } = new();

    public string TitleName => Server.Id is null ? "New Server" : Server.Name!;
    public bool IsServerCreated => Server.Id is not null;
    public DateTime? CreationDate => Server is null ? null : Server.CreationDate;
    [ObservableProperty]
    public partial ImageSource ServerIcon { get; set; } = ImageSource.FromFile("pack.png");
    [ObservableProperty]
    public partial bool ServerIconIsPointerOver { get; set; } = false;

    //Versions
    [ObservableProperty]
    public partial List<MinecraftVersionInfo> MinecraftVersionsInfos { get; set; } = new List<MinecraftVersionInfo>();
    [ObservableProperty]
    public partial ObservableCollection<KeyValuePair<JavaInstallation?, string>> JavaInstallationOptions { get; set; } = new();
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
    public ObservableCollection<ServerPropertyQuery> ServerProperties { get; set; } = [];
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredServerProperties))]
    public partial string PropertySearchText { get; set; } = string.Empty;
    public IReadOnlyList<ServerPropertyQuery> FilteredServerProperties =>
        string.IsNullOrWhiteSpace(PropertySearchText)
            ? ServerProperties
            : ServerProperties
                .Where(property =>
                    property.Definition.Key.Contains(PropertySearchText.Trim(), StringComparison.OrdinalIgnoreCase) ||
                    property.Definition.DisplayName.Contains(PropertySearchText.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();

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
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredMods))]
    [NotifyPropertyChangedFor(nameof(ModEmptyTitle))]
    [NotifyPropertyChangedFor(nameof(ModEmptyMessage))]
    public partial string ModSearchText { get; set; } = string.Empty;
    public string ModEmptyTitle => string.IsNullOrWhiteSpace(ModSearchText) ? "No mods added" : "No matching mods";
    public string ModEmptyMessage => string.IsNullOrWhiteSpace(ModSearchText)
        ? "Added mods will appear here."
        : "Try a different mod name.";
    public IReadOnlyList<ModCardViewModel> FilteredMods =>
        string.IsNullOrWhiteSpace(ModSearchText)
            ? Mods
            : Mods
                .Where(mod => mod.Name?.Contains(ModSearchText.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                .ToList();


    /***************
    * CONSTRUCTORS *
    ***************/
    public ServerViewModel(IMinecraftCatalogService minecraftCatalogService, IServerManagerService serverHandlerService, IModloaderCatalogService modloaderCatalogService, IModCatalogService modCatalogService, IJavaManagerService javaManagerService)
    {
        this.minecraftCatalogService = minecraftCatalogService;
        this.serverHandlerService = serverHandlerService;
        this.modloaderCatalogService = modloaderCatalogService;
        this.modCatalogService = modCatalogService;
        this.javaManagerService = javaManagerService;

        ServerProperties.CollectionChanged += (_, _) => OnPropertyChanged(nameof(FilteredServerProperties));
        Mods.CollectionChanged += (_, _) => OnPropertyChanged(nameof(FilteredMods));
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

        var javaInstallations = (await javaManagerService.GetJavaInstallationsAsync()).ToList();
        JavaInstallationOptions = new ObservableCollection<KeyValuePair<JavaInstallation?, string>>
        {
            new(null, "Automatic (recommended)")
        };
        foreach (var installation in javaInstallations)
            JavaInstallationOptions.Add(new(installation, $"{installation.Name} (Java {installation.Version})"));

        if (server.JavaInstallation is not null)
            server.JavaInstallation = javaInstallations.FirstOrDefault(j => j.Matches(server.JavaInstallation));


        //Obterner versiones de MC
        IncludeSnapshots = server.MinecraftVersionBase?.MinecraftVersionType == MinecraftVersionType.Snapshot;
        await RequestMinecraftVersionsAsync();

        //Obtener server.property definitions
        await RequestServerPropertiesAsync(server);

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
        pendingServerIcon = null;
        ServerIcon = server.Id is Guid serverId && serverHandlerService.GetServerIconPath(serverId) is string iconPath
            ? ImageSource.FromStream(() => File.OpenRead(iconPath))
            : ImageSource.FromFile("pack.png");
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

    private async Task RequestServerPropertiesAsync(ServerQuery server)
    {
        ServerPropertiesStatus = "Loading";

        var definitions = (await minecraftCatalogService.GetServerPropertyDefinitionsAsync())
            .ToDictionary(definition => definition.Key);

        GamemodeServerProperty = definitions["gamemode"];
        DifficultyServerProperty = definitions["difficulty"];
        MotdServerProperty = definitions["motd"];
        ServerIpServerProperty = definitions["server-ip"];
        MaxPlayersServerProperty = definitions["max-players"];

        ServerProperties.Clear();
        foreach (var property in server.Properties)
        {
            if (!definitions.TryGetValue(property.Key, out var definition))
                continue;

            ServerProperties.Add(new ServerPropertyQuery
            {
                Server = server,
                Definition = definition
            });
        }

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
        ResetModloaderVersions();

        string? minecraftVersion = Server.MinecraftVersionBase?.MinecraftVersion;
        if (minecraftVersion is null) return;
        if (ModloaderTypeQuery.ModloaderType is null) return;

        ModloaderType modloaderType = (ModloaderType)ModloaderTypeQuery.ModloaderType;

        var modloaderInfos = await modloaderCatalogService.GetModloaderVersionsAsync(
                modloaderType,
                minecraftVersion
            );

        foreach (var modloaderInfo in modloaderInfos)
        {
            ModloaderInfos.Add(modloaderInfo);
        }
        ModloaderInfoIsVisible = true;
    }

    [RelayCommand]
    private void ResetModloaderVersions()
    {
        ModloaderInfoIsVisible = false;
        ModloaderInfos.Clear();
        SelectedModloaderInfo = null;
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
        Guid serverId;
        bool changesPopupWasShown = false;
        if (Server.Id is not null)
        {
            ServerChanges? changes = await serverHandlerService.EditServerAsync(Server);
            if (changes is not null)
            {
                changesPopupWasShown = true;
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

                if (pendingServerIcon is null) return;
            }
            serverId = (Guid)Server.Id;
        }
        else
        {
            ServerQuery createdServer = await serverHandlerService.CreateServerAsync(Server);
            serverId = (Guid)createdServer.Id!;
        }

        if (pendingServerIcon is not null)
        {
            using MemoryStream image = new(pendingServerIcon);
            await serverHandlerService.SetServerIconAsync(serverId, image);
        }

        if (changesPopupWasShown) return;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task SelectServerIconAsync()
    {
        FileResult? result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select a server icon",
            FileTypes = FilePickerFileType.Images
        });
        if (result is null) return;

        await using Stream image = await result.OpenReadAsync();
        using MemoryStream buffer = new();
        await image.CopyToAsync(buffer);
        pendingServerIcon = buffer.ToArray();
        ServerIcon = ImageSource.FromStream(() => new MemoryStream(pendingServerIcon));
    }

    [RelayCommand]
    private void ShowServerIconOverlay()
    {
        ServerIconIsPointerOver = true;
    }

    [RelayCommand]
    private void HideServerIconOverlay()
    {
        ServerIconIsPointerOver = false;
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

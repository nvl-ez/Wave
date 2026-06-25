using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;
using Wave.Domain.Utils;
using Wave.Ui.Utils;

namespace Wave.Ui.Pages.ServerContent.ViewModels;

public partial class ModsPopupViewModel : ObservableObject
{
    private readonly IModCatalogService modCatalogService;

    [ObservableProperty]
    public partial ServerQuery Server { get; set; }

    //MOD INFOS
    public ObservablePaginationState ObservablePaginationState { get; set; } = new ObservablePaginationState(new PaginationState() { Index = 0 });
    public ObservableCollection<ModCardViewModel> ModInfos { get; } = [];

    //MOD DETAILS
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModDescription))]
    [NotifyPropertyChangedFor(nameof(ModDescriptionType))]
    [NotifyPropertyChangedFor(nameof(ModName))]
    [NotifyPropertyChangedFor(nameof(ModIconUrl))]
    public partial ModDetails? ModDetails { get; set; } = null;

    public string? ModName => ModDetails?.ModName;
    public string? ModIconUrl => ModDetails?.IconUrl;
    public string? ModDescription => ModDetails?.ModDescription;
    public string? ModDescriptionType
    {
        get
        {
            if (ModDetails is null) return "Loading";
            if (ModDetails.ModDescriptionType == Domain.Mods.ModDescriptionType.Html) return "Html";
            else return "Text";
        }
    }


    //MOD VERSIONS
    public ObservableCollection<ModVersion> ModVersions { get; } = [];

    //MOD SUPPLIERS
    public ObservableCollection<KeyValuePair<ModSupplierType, string>> ModSupplierTypes { get; } = [];

    public ModInfoSupplierQuery ModInfoSupplierQuery { get; }

    //MOD FIES
    public ObservableCollection<ModFile> ModFiles { get; } = [];
    public ModVersion? CurrentModVersion { get; set; } = null;
    public ModBase? CurrentMod { get; set; } = null;


    public ModsPopupViewModel(ServerQuery server, IModCatalogService modCatalogService)
    {
        Server = server;
        this.modCatalogService = modCatalogService;

        //Todo: error si no hay version ni modloader
        ModInfoSupplierQuery = new ModInfoSupplierQuery()
        {
            MinecraftVersion = Server.MinecraftVersionBase!.MinecraftVersion,
            ModloaderType = Server.Modloader!.ModloaderType,
            ModSupplierType = default,
            PaginationState = ObservablePaginationState.AsPaginationState(),
            TextQuery = "",
            Author = ""
        };

        foreach (var modFile in server.Mods)
        {
            ModFiles.Add(modFile);
        }
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        ModSupplierTypes.Clear();
        var modSupplierTypes = await modCatalogService.GetModSupplierTypesAsync();

        foreach (var kv in modSupplierTypes)
        {
            ModSupplierTypes.Add(kv);
        }
    }

    [RelayCommand]
    public async Task PaginatedSearchModsAsync()
    {
        if (Server.Modloader is null || Server.MinecraftVersionBase is null) return;

        //Actualizar pagination state
        ModInfoSupplierQuery.PaginationState = ObservablePaginationState.AsPaginationState();

        var response = await modCatalogService.SearchModsAsync(ModInfoSupplierQuery);
        ObservablePaginationState.Apply(response.PaginationState);
        ModInfos.Clear();

        foreach (var modInfo in response.Mods)
        {
            ModInfos.Add(new ModCardViewModel(modInfo));
        }
    }

    [RelayCommand]
    public async Task SearchModsAsync()
    {
        if (Server.Modloader is null || Server.MinecraftVersionBase is null) return;

        ObservablePaginationState.Index = 0;
        ObservablePaginationState.TotalCount = 0;
        ObservablePaginationState.ResultCount = 0;

        //Actualizar pagination state
        ModInfoSupplierQuery.PaginationState = ObservablePaginationState.AsPaginationState();

        var response = await modCatalogService.SearchModsAsync(ModInfoSupplierQuery);
        ObservablePaginationState.Apply(response.PaginationState);
        ModInfos.Clear();

        foreach (var modInfo in response.Mods)
        {
            ModInfos.Add(new ModCardViewModel(modInfo));
        }
    }

    [RelayCommand]
    public async Task ClosePopupAsync()
    {
        await Shell.Current.ClosePopupAsync();
    }

    [RelayCommand]
    public async Task GetModDetailsWithModInfoAsync(ModInfo modInfo)
    {
        if (modInfo is null) return;

        CurrentMod = modInfo;

        var query = new ModVersionSupplierQuery()
        {
            MinecraftVersion = Server.MinecraftVersionBase!.MinecraftVersion,
            ModId = modInfo.ModId,
            ModloaderType = Server.Modloader!.ModloaderType,
            ModSupplierType = modInfo.ModSupplierType
        };

        await GetModDetailsAndVersions(query);
    }

    [RelayCommand]
    public async Task GetModDetailsWithModFileAsync(ModFile modFile)
    {
        if (modFile is null) return;

        CurrentMod = modFile;

        var query = new ModVersionSupplierQuery()
        {
            MinecraftVersion = Server.MinecraftVersionBase!.MinecraftVersion,
            ModId = modFile.ModId,
            ModloaderType = Server.Modloader!.ModloaderType,
            ModSupplierType = modFile.ModSupplierType
        };

        await GetModDetailsAndVersions(query);
    }

    private async Task GetModDetailsAndVersions(ModVersionSupplierQuery query)
    {
        if (CurrentMod is null) return;

        ModDetails = await modCatalogService.GetModDetailsAsync(CurrentMod, query.ModSupplierType);

        ModVersions.Clear();

        var modVersions = await modCatalogService.GetModVersionsAsync(query);

        foreach (var modVersion in modVersions.Versions)
        {
            ModVersions.Add(modVersion);
        }
    }

    [RelayCommand]
    public void AddModFile()
    {
        if (CurrentModVersion is null) return;
        if (CurrentMod is null) return;

        var modFile = new ModFile(CurrentMod, CurrentModVersion);

        //Elimina si existe una version diferente del mod
        RemoveModFile(modFile.ModId);

        Server.Mods = Server.Mods.Append(modFile);
        ModFiles.Add(modFile);
    }

    [RelayCommand]
    public void RemoveModFile(string modId)
    {
        Server.Mods = Server.Mods.Where(m => m.ModId != modId);

        var modFile = ModFiles.FirstOrDefault(m => m.ModId == modId);
        if (modFile is not null) ModFiles.Remove(modFile);
    }
}

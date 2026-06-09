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
    public ObservableCollection<ModCardViewModel> ModInfos { get; set; } = new();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModName))]
    [NotifyPropertyChangedFor(nameof(ModIconUrl))]
    public partial ModInfo? SelectedModInfo { get; set; } = null;

    //MOD DETAILS
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ModDescription))]
    [NotifyPropertyChangedFor(nameof(ModDescriptionType))]
    public partial ModDetails? ModDetails { get; set; } = null;

    public string? ModName => SelectedModInfo?.Name;
    public string? ModIconUrl => SelectedModInfo?.IconUrl;
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
    public ObservableCollection<ModVersion> ModVersions { get; set; } = new();

    //MOD SUPPLIERS
    [ObservableProperty]
    public partial ObservableCollection<KeyValuePair<ModSupplierType, string>> ModSupplierTypes { get; set; } = [];

    public ModInfoSupplierQuery ModInfoSupplierQuery { get; }


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
        SelectedModInfo = modInfo;
        ModDetails = await modCatalogService.GetModDetailsAsync(modInfo.ModId, modInfo.ModSupplierType);
    }

    [RelayCommand]
    public async Task GetModDetailsWithModFileAsync(ModFile modFile)
    {
        if (modFile is null) return;
        ModDetails = await modCatalogService.GetModDetailsAsync(modFile.ModId, modFile.ModSupplierType);
    }


    private async Task GetModDetailsAndVersions(ModVersionSupplierQuery query, ModSupplierType modSupplierType)
    {
        ModDetails = await modCatalogService.GetModDetailsAsync(query.ModId, modSupplierType);

        ModVersions.Clear();

        var modVersions = await modCatalogService.GetModVersionsAsync(query);

        foreach (var modVersion in modVersions.Versions)
        {
            ModVersions.Add(modVersion);
        }
    }
}

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.Configuration;

namespace Wave.Ui.Pages.SettingsContent.ViewModels;

public partial class SettingsViewModel : ObservableObject, IQueryAttributable
{
    private readonly IApplicationConfigurationService configurationService;
    /***************************
    * VARIABLES AND PROPERTIES *
    ****************************/
    //Pseudo Navigation
    [ObservableProperty]
    public partial string CurrentTab { get; set; } = "General";

    [ObservableProperty]
    public partial ApplicationConfiguration Configuration { get; set; } = new();
    public SettingsViewModel(IApplicationConfigurationService configurationService)
    {
        this.configurationService = configurationService;
    }
    /***************
    * CONSTRUCTORS *
    ***************/


    /**********
    * METHODS *
    **********/
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        return;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Configuration = await configurationService.GetAsync();
    }

    [RelayCommand]
    private Task SaveConfigurationAsync(ApplicationConfiguration configuration) =>
        configurationService.SaveAsync(configuration);

    [RelayCommand]
    private void ShowGeneralView() => CurrentTab = "General";

    [RelayCommand]
    private void ShowJavaView() => CurrentTab = "Java";
}

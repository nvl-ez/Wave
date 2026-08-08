using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Domain.Configuration;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;
using System.Collections.ObjectModel;

namespace Wave.Ui.Pages.SettingsContent.ViewModels;

public partial class SettingsViewModel : ObservableObject, IQueryAttributable
{
    private readonly IApplicationConfigurationService configurationService;
    private readonly IJavaManagerService javaManagerService;
    private readonly IServerManagerService serverManagerService;
    /***************************
    * VARIABLES AND PROPERTIES *
    ****************************/
    //Pseudo Navigation
    [ObservableProperty]
    public partial string CurrentTab { get; set; } = "General";

    [ObservableProperty]
    public partial ApplicationConfiguration Configuration { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<KeyValuePair<JavaInstallation?, string>> JavaInstallationOptions { get; set; } = [];

    public SettingsViewModel(IApplicationConfigurationService configurationService, IJavaManagerService javaManagerService, IServerManagerService serverManagerService)
    {
        this.configurationService = configurationService;
        this.javaManagerService = javaManagerService;
        this.serverManagerService = serverManagerService;
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
        ApplicationConfiguration configuration = await configurationService.GetAsync();
        var installations = (await javaManagerService.GetJavaInstallationsAsync()).ToList();
        JavaInstallationOptions = [new(null, "Automatic")];
        foreach (JavaInstallation installation in installations)
            JavaInstallationOptions.Add(new(installation, $"{installation.Name} (Java {installation.Version})"));

        if (configuration.JavaInstallation is not null)
            configuration.JavaInstallation = installations.FirstOrDefault(i => i.Matches(configuration.JavaInstallation));

        Configuration = configuration;
    }

    [RelayCommand]
    private async Task SaveConfigurationAsync(ApplicationConfiguration configuration)
    {
        await configurationService.SaveAsync(configuration);
        await serverManagerService.SetJavaInstallationForAllAsync(configuration.JavaInstallation);
    }

    [RelayCommand]
    private void ShowGeneralView() => CurrentTab = "General";

    [RelayCommand]
    private void ShowJavaView() => CurrentTab = "Java";
}

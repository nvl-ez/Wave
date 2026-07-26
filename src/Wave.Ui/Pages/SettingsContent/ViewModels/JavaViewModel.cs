using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.In;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.System;

namespace Wave.Ui.Pages.SettingsContent.ViewModels;

public partial class JavaViewModel : ObservableObject
{
    /***************************
    * VARIABLES AND PROPERTIES *
    ****************************/
    private readonly IDeviceInformationService deviceInformationService;
    private readonly IJavaManagerService javaManagerService;

    //State
    [ObservableProperty]
    public partial bool QuerySectionIsVisible { get; set; } = false;
    [ObservableProperty]
    public partial string QuerySectionState { get; set; } = "Loading";
    [ObservableProperty]
    public partial bool VersionsSectionIsVisible { get; set; } = false;
    [ObservableProperty]
    public partial string VersionsSectionState { get; set; } = "Loading"; //Loading, Loaded
    [ObservableProperty]
    public partial bool ArtifactsSectionIsVisible { get; set; } = false;
    [ObservableProperty]
    public partial bool InstallSectionIsVisible { get; set; } = false;
    [ObservableProperty]
    public partial bool IsInstallButtonEnabled { get; set; } = true;

    //Lists
    [ObservableProperty]
    public partial ObservableCollection<JavaInstallation> JavaInstallations { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<IJavaSupplier> JavaSuppliers { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<JavaVersion> JavaVersions { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<int> JavaVersionNumbers { get; set; } = new();
    [ObservableProperty]
    public partial ObservableCollection<JavaArtifact> JavaArtifacts { get; set; } = new();

    //Picked Values
    public IJavaSupplier? SelectedJavaSupplier { get; set; } = null;
    public int? SelectedJavaVersionNumber { get; set; } = null;
    public JavaVersion? SelectedJavaVersion { get; set; } = null;
    public JavaArtifact? SelectedJavaArtifact { get; set; } = null;

    /***************
    * CONSTRUCTORS *
    ***************/
    public JavaViewModel(IDeviceInformationService deviceInformationService, IJavaManagerService javaManagerService)
    {
        this.deviceInformationService = deviceInformationService;
        this.javaManagerService = javaManagerService;
    }

    /**********
    * METHODS *
    **********/
    [RelayCommand]
    public async Task LoadAsync()
    {
        ResetPage();

        JavaInstallations = new ObservableCollection<JavaInstallation>(await javaManagerService.GetJavaInstallationsAsync());
        JavaSuppliers = new ObservableCollection<IJavaSupplier>(javaManagerService.GetJavaSuppliers());
    }

    [RelayCommand]
    public async Task JavaSupplierSelected()
    {
        if (SelectedJavaSupplier is null) return;

        IJavaSupplier selectedJavaSupplier = SelectedJavaSupplier;
        ResetSupplierDependentState();

        QuerySectionIsVisible = true;
        QuerySectionState = "Loading";

        DeviceInformation deviceInformation = deviceInformationService.GetDeviceInformation();

        var responseVersions = await javaManagerService.GetAvailableMajorVersionsAsync(selectedJavaSupplier, deviceInformation.Os);

        // Ignore a response that belongs to a supplier that is no longer selected.
        if (!ReferenceEquals(SelectedJavaSupplier, selectedJavaSupplier)) return;

        JavaVersionNumbers = new ObservableCollection<int>(responseVersions);
        QuerySectionState = "Loaded";
    }

    [RelayCommand]
    public async Task QueryVersions()
    {
        if (SelectedJavaVersionNumber is null || SelectedJavaSupplier is null) return;
        VersionsSectionIsVisible = true;
        VersionsSectionState = "Loading";

        DeviceInformation deviceInformation = deviceInformationService.GetDeviceInformation();

        var responseVersions = await javaManagerService.GetJavaVersionsAsync(SelectedJavaSupplier,
        new JavaSupplierQuery()
        {
            ArchitectureBitType = deviceInformation.ArchitectureBit,
            ArchitectureType = deviceInformation.Architecture,
            OsType = deviceInformation.Os,
            Version = (int)SelectedJavaVersionNumber
        });
        JavaVersions = new ObservableCollection<JavaVersion>(responseVersions);
        VersionsSectionState = "Loaded";
    }

    [RelayCommand]
    public void JavaVersionSelected()
    {
        if (SelectedJavaVersionNumber is null || SelectedJavaSupplier is null || SelectedJavaVersion is null) return;

        JavaArtifacts.Clear();

        foreach (var artifact in SelectedJavaVersion!.JavaArtifacts)
        {
            JavaArtifacts.Add(artifact);
        }

        ArtifactsSectionIsVisible = true;
    }

    [RelayCommand]
    public void JavaArtifactSelected()
    {
        if (SelectedJavaVersionNumber is null || SelectedJavaSupplier is null || SelectedJavaVersion is null || SelectedJavaArtifact is null) return;

        InstallSectionIsVisible = true;
    }
    [RelayCommand]
    public async Task DownloadAndInstall()
    {
        if (SelectedJavaVersionNumber is null || SelectedJavaSupplier is null || SelectedJavaVersion is null || SelectedJavaArtifact is null) return;

        bool isAlreadyInstalled = JavaInstallations.Any(javaInstallation =>
            javaInstallation.JavaSupplierType == SelectedJavaVersion.JavaSupplierType &&
            javaInstallation.Version == SelectedJavaVersion.Version);

        if (isAlreadyInstalled)
        {
            await Shell.Current.DisplayAlertAsync(
                "Java already installed",
                $"{SelectedJavaVersion.Name} from {SelectedJavaVersion.JavaSupplierType} cannot be installed because that supplier and version are already installed.",
                "OK");
            return;
        }

        IsInstallButtonEnabled = true;
        JavaInstallation javaInstallation = await javaManagerService.InstallJavaVersionAsync(SelectedJavaVersion!, SelectedJavaArtifact!);
        ResetPage();
        JavaInstallations.Add(javaInstallation);
    }

    private void ResetPage()
    {
        SelectedJavaSupplier = null;
        ResetSupplierDependentState();

        QuerySectionIsVisible = false;
        IsInstallButtonEnabled = true;
    }

    private void ResetSupplierDependentState()
    {
        SelectedJavaVersionNumber = null;
        SelectedJavaVersion = null;
        SelectedJavaArtifact = null;

        JavaVersionNumbers.Clear();
        JavaVersions.Clear();
        JavaArtifacts.Clear();

        QuerySectionState = "Loading";
        VersionsSectionIsVisible = false;
        VersionsSectionState = "Loading";
        ArtifactsSectionIsVisible = false;
        InstallSectionIsVisible = false;
    }
}

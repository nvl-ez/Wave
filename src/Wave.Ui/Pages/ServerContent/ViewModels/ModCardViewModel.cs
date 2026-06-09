using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Wave.Domain.Mods;

namespace Wave.Ui.Pages.ServerContent.ViewModels;

public partial class ModCardViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Image))]
    [NotifyPropertyChangedFor(nameof(Name))]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial ModInfo ModInfo { get; set; }
    [ObservableProperty]
    public partial ModVersion? ModVersion { get; set; }

    public string? Image => ModInfo.IconUrl;
    public string? Name => ModInfo.Name;
    public string? Summary => ModInfo.Summary;

    public bool IsSelected { get; set; } = false;

    public ModCardViewModel(ModInfo modInfo)
    {
        ModInfo = modInfo;
    }

    public ModCardViewModel(ModInfo modInfo, ModVersion modVersion)
    {
        ModInfo = modInfo;
        ModVersion = modVersion;
    }
}

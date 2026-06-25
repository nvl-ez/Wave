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
    public partial ModInfo? ModInfo { get; set; } = null;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Image))]
    [NotifyPropertyChangedFor(nameof(Name))]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial ModFile? ModFile { get; set; } = null;

    public string? Image => ModFile is null ? ModInfo!.IconUrl : ModFile.IconUrl;
    public string? Name => ModFile is null ? ModInfo!.ModName : ModFile.ModName;
    public string? Summary => ModFile is null ? ModInfo!.ModSummary : ModFile.ModSummary;

    public bool IsSelected { get; set; } = false;

    public ModCardViewModel(ModInfo modInfo)
    {
        ModInfo = modInfo;
    }

    public ModCardViewModel(ModFile modFile)
    {
        ModFile = modFile;
    }
}

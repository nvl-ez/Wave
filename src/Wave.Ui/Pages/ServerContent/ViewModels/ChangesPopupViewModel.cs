using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Ui.Pages.ServerContent.ViewModels;

public partial class ChangesPopupViewModel : ObservableObject
{
    public ModloaderBase? DeletedModloader { get; }
    public ObservableCollection<ModCardViewModel> DeletedMods { get; } = [];
    public ObservableCollection<ModCardViewModel> FailedMods { get; } = [];

    public bool HasDeletedModloader => DeletedModloader is not null;
    public bool HasDeletedMods => DeletedMods.Count > 0;
    public bool HasFailedMods => FailedMods.Count > 0;

    public ChangesPopupViewModel(ServerChanges changes)
    {
        DeletedModloader = changes.DeletedModloader;

        foreach (var mod in changes.DeletedMods ?? [])
        {
            DeletedMods.Add(new ModCardViewModel(mod));
        }

        foreach (var mod in changes.FailedMods ?? [])
        {
            FailedMods.Add(new ModCardViewModel(mod));
        }
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        await Shell.Current.ClosePopupAsync();
        await Shell.Current.GoToAsync("..");
    }
}

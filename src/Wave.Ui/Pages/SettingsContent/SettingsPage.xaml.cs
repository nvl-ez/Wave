using Wave.Ui.Pages.SettingsContent.ViewModels;

namespace Wave.Ui.Pages.SettingsContent;

public partial class SettingsPage : ContentPage
{
	private static readonly SettingsViewModel settingsViewModel = AppComposition.CreateSettingsViewModel();
	public SettingsPage()
	{
		InitializeComponent();
		BindingContext = settingsViewModel;
	}
}
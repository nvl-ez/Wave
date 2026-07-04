using Wave.Domain.ServerManager;
using Wave.Ui.Pages.ServerContent.ViewModels;

namespace Wave.Ui.Pages.ServerContent.Views;

public partial class ChangesPopup : ContentView
{
	public ChangesPopup(ServerChanges changes)
	{
		InitializeComponent();
		BindingContext = new ChangesPopupViewModel(changes);
	}
}

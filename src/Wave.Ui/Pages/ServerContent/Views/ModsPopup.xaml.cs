using Wave.Application.In;
using Wave.Domain.ServerManager;
using Wave.Ui.Pages.ServerContent.ViewModels;

namespace Wave.Ui.Pages.ServerContent.Views;

public partial class ModsPopup : ContentView
{
	public ModsPopup(ServerQuery server, IModCatalogService modCatalogService)
	{
		InitializeComponent();
		BindingContext = new ModsPopupViewModel(server, modCatalogService);
	}
}
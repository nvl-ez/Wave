using Wave.Infrastructure.Out.ServerManager;
using Wave.Ui.ViewModels;

namespace Wave.Ui.Pages;

public partial class ServersPage : ContentPage
{
	//TODO Mover a AppComposition
	private static readonly ServersViewModel serversViewModel = AppComposition.CreateServersViewModel();
	public ServersPage()
	{
		InitializeComponent();
		BindingContext = serversViewModel;
	}
}
using Wave.Ui.Pages.ServersContent.ViewModels;

namespace Wave.Ui.Pages.ServersContent;

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
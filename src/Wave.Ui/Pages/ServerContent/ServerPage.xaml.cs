using Wave.Ui.Pages.ServerContent.ViewModels;

namespace Wave.Ui.Pages.ServerContent;

public partial class ServerPage : ContentPage
{
	//TODO Mover a AppComposition
	private static readonly ServerViewModel serverViewModel = AppComposition.CreateServerViewModel();
	public ServerPage()
	{
		InitializeComponent();
		BindingContext = serverViewModel;
	}
}
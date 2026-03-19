using Wave.Infrastructure.Out.Minecraft.Api;
using Wave.Infrastructure.Out.ServerManager;
using Wave.Ui.ViewModels;

namespace Wave.Ui.Pages;

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
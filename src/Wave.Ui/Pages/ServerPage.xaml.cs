using Wave.Application.Services;
using Wave.Infrastructure.Out.Minecraft.Api;
using Wave.Infrastructure.Out.ServerManager;
using Wave.Ui.ViewModels;

namespace Wave.Ui.Pages;

public partial class ServerPage : ContentPage
{
	//TODO Mover a AppComposition
	private static readonly ServerViewModel serverViewModel = new ServerViewModel(new ServerJsonRepository("C:\\Users\\nahu\\Documents\\JavaTest"), new MinecraftVersionCatalogService(new MinecraftVersionCatalog()));
	public ServerPage()
	{
		InitializeComponent();
		BindingContext = serverViewModel;
	}
}
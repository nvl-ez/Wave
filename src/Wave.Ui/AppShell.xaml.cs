using Wave.Ui.Pages;

namespace Wave.Ui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(ServerPage), typeof(ServerPage));
	}
}

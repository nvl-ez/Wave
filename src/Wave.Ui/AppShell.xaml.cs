using Wave.Ui.Pages;
using Wave.Ui.Pages.ServerContent;
using Wave.Ui.Pages.ExecutionContent;

namespace Wave.Ui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(ServerPage), typeof(ServerPage));
		Routing.RegisterRoute(nameof(ExecutionPage), typeof(ExecutionPage));
	}
}

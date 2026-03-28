using Wave.Ui.Pages.SettingsContent.ViewModels;

namespace Wave.Ui.Pages.SettingsContent.Views;

public partial class JavaView : ContentView
{
	private static readonly JavaViewModel javaViewModel = AppComposition.CreateJavaViewModel();
	public JavaView()
	{
		InitializeComponent();
		BindingContext = javaViewModel;
	}
}
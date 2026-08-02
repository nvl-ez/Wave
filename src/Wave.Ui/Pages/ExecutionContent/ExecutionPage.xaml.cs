using Wave.Application.In;
using Wave.Ui.Pages.ExecutionContent.ViewModels;


namespace Wave.Ui.Pages.ExecutionContent;

public partial class ExecutionPage : ContentPage
{
	private static readonly ExecutionViewModel executionViewModel = AppComposition.CreateExecutionViewModel();
	public ExecutionPage()
	{
		InitializeComponent();
		BindingContext = executionViewModel;
	}
}

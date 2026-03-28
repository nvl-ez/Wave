using Wave.Application.In;


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
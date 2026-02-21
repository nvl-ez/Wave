namespace Wave.Ui;

public partial class AllNotesPage : ContentPage
{
	public AllNotesPage()
	{
		InitializeComponent();

		BindingContext = AppComposition.CreateNotesViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		notesCollection.SelectedItem = null;
	}

}
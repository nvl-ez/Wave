namespace Wave.Ui;

public partial class NotePage : ContentPage
{

	public NotePage()
	{
		InitializeComponent();
		BindingContext = AppComposition.CreateNoteViewModel();
	}
}
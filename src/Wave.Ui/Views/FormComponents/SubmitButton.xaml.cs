namespace Wave.Ui.Views.FormComponents;

public partial class SubmitButton : Button, IFormElement
{
	private Form? form = null;
	public bool IsRequired { get; set; }

	public object? Model { get; private set; }

	public SubmitButton()
	{
		InitializeComponent();
	}

	public void SetInvalid() { }
	public void SetValid() { }

	public bool Validate()
	{
		return true;
	}

	private void OnClicked(object sender, EventArgs e)
	{
		form?.Submit();
	}

	private void OnParentChanged(object sender, EventArgs e)
	{
		FindParent();
	}

	private void Button_Loaded(object sender, EventArgs e)
	{
		FindParent();
	}

	private void FindParent()
	{
		Element? current = this;

		while (current is not null)
		{
			if (current is Form parentForm)
			{
				form = parentForm;
				break;
			}

			current = current.Parent;
		}
	}

	public void SetModel(object? model)
	{
		Model = model;
	}
}
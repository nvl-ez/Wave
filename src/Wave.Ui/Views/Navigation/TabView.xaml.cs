namespace Wave.Ui.Views.Navigation;

public partial class TabView : Button
{
	private System.Windows.Input.ICommand? availableCommand;

	public static readonly BindableProperty IsAvailableProperty = BindableProperty.Create(
		nameof(IsAvailable),
		typeof(bool),
		typeof(TabView),
		true,
		propertyChanged: OnIsAvailableChanged);

	public bool IsAvailable
	{
		get => (bool)GetValue(IsAvailableProperty);
		set => SetValue(IsAvailableProperty, value);
	}

	public TabView()
	{
		InitializeComponent();
	}

	private static void OnIsAvailableChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var tab = (TabView)bindable;
		var isAvailable = (bool)newValue;

		tab.Opacity = isAvailable ? 1 : 0.45;
		if (isAvailable)
		{
			tab.Command = tab.availableCommand;
			tab.availableCommand = null;
		}
		else
		{
			tab.availableCommand = tab.Command;
			tab.Command = null;
		}
	}
}

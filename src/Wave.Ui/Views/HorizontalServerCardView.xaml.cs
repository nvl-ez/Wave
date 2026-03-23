using System.Windows.Input;

namespace Wave.Ui.Views;

public partial class HorizontalServerCardView : ContentView
{
	public static readonly BindableProperty EditServerCommandProperty =
		BindableProperty.Create(nameof(EditServerCommand), typeof(ICommand), typeof(HorizontalServerCardView));
	public static readonly BindableProperty StartServerCommandProperty =
		BindableProperty.Create(nameof(StartServerCommand), typeof(ICommand), typeof(HorizontalServerCardView));

	public static readonly BindableProperty ServerCommandParameterProperty =
		BindableProperty.Create(nameof(ServerCommandParameter), typeof(object), typeof(HorizontalServerCardView));

	public ICommand? EditServerCommand
	{
		get => (ICommand?)GetValue(EditServerCommandProperty);
		set => SetValue(EditServerCommandProperty, value);
	}

	public ICommand? StartServerCommand
	{
		get => (ICommand?)GetValue(StartServerCommandProperty);
		set => SetValue(StartServerCommandProperty, value);
	}

	public object? ServerCommandParameter
	{
		get => GetValue(ServerCommandParameterProperty);
		set => SetValue(ServerCommandParameterProperty, value);
	}

	public HorizontalServerCardView()
	{
		InitializeComponent();
	}
}
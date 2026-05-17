using CommunityToolkit.Mvvm.ComponentModel;
using Wave.Domain.Minecraft;

namespace Wave.Ui.Pages.ServerContent.Views;

public partial class ServerPropertyInputView : ContentView
{
	public static readonly BindableProperty ServerPropertyDefinitionProperty =
		BindableProperty.Create(nameof(ServerPropertyDefinition), typeof(PropertyDefinition), typeof(ServerPropertyInputView), propertyChanged: OnServerPropertyDefinitionChanged);

	public static readonly BindableProperty ServerPropertyValueProperty =
		BindableProperty.Create(nameof(ServerPropertyValue), typeof(string), typeof(ServerPropertyInputView), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnServerPropertyValueChanged);

	public PropertyDefinition? ServerPropertyDefinition
	{
		get => (PropertyDefinition?)GetValue(ServerPropertyDefinitionProperty);
		set => SetValue(ServerPropertyDefinitionProperty, value);
	}
	public string? ServerPropertyValue
	{
		get => (string?)GetValue(ServerPropertyValueProperty);
		set => SetValue(ServerPropertyValueProperty, value);
	}

	public string? ServerPropertyType => ServerPropertyDefinition?.Type.ToString();

	public List<KeyValuePair<string, string>>? Options => ServerPropertyDefinition?.Options?.ToList();

	public int ListIndex
	{
		get => ServerPropertyValue is null || Options is null
			? -1
			: Options.FindIndex(kv => kv.Key == ServerPropertyValue);

		set
		{
			if (Options is null || value < 0 || value >= Options.Count)
				return;

			ServerPropertyValue = Options[value].Key;
		}
	}
	//Values
	public bool? IsChecked
	{
		get
		{
			if (string.IsNullOrEmpty(ServerPropertyValue) || ServerPropertyValue == "false") return false;
			else return true;
		}
		set
		{
			if (ServerPropertyType == "Boolean")
			{
				if (value is null || value == false) ServerPropertyValue = "false";
				else ServerPropertyValue = "true";
			}
		}
	}

	public string? StringValue
	{
		get
		{
			return ServerPropertyValue;
		}
		set
		{
			if (ServerPropertyType == "String" || ServerPropertyType == "Integer" || ServerPropertyDefinition is null)
			{
				ServerPropertyValue = value;
			}
		}
	}

	public ServerPropertyInputView()
	{
		InitializeComponent();
	}

	static void OnServerPropertyValueChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var view = (ServerPropertyInputView)bindable;
		view.OnPropertyChanged(nameof(StringValue));
		view.OnPropertyChanged(nameof(IsChecked));
		view.OnPropertyChanged(nameof(ListIndex));
	}

	static void OnServerPropertyDefinitionChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var view = (ServerPropertyInputView)bindable;
		view.OnPropertyChanged(nameof(ServerPropertyType));
		view.OnPropertyChanged(nameof(Options));
		view.OnPropertyChanged(nameof(StringValue));
		view.OnPropertyChanged(nameof(IsChecked));
		view.OnPropertyChanged(nameof(ListIndex));
	}
}
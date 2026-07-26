using Wave.Domain.ServerManager.Properties;
using Wave.Ui.Views.FormComponents;

namespace Wave.Ui.Pages.ServerContent.Views;

public partial class ServerPropertyInputView : ContentView, IFormElement
{
	private bool syncingFromModel;

	public static readonly BindableProperty ServerPropertyDefinitionProperty =
		BindableProperty.Create(
			nameof(ServerPropertyDefinition),
			typeof(PropertyDefinition),
			typeof(ServerPropertyInputView),
			propertyChanged: OnServerPropertyDefinitionChanged);

	public static readonly BindableProperty ServerPropertyKeyProperty =
		BindableProperty.Create(
			nameof(ServerPropertyKey),
			typeof(string),
			typeof(ServerPropertyInputView),
			propertyChanged: OnModelPathConfigChanged);

	public static readonly BindableProperty PropertyDictionaryModelPathProperty =
		BindableProperty.Create(
			nameof(PropertyDictionaryModelPath),
			typeof(string),
			typeof(ServerPropertyInputView),
			"Properties",
			propertyChanged: OnModelPathConfigChanged);

	public static readonly BindableProperty ServerPropertyValueProperty =
		BindableProperty.Create(
			nameof(ServerPropertyValue),
			typeof(string),
			typeof(ServerPropertyInputView),
			default(string),
			BindingMode.TwoWay,
			propertyChanged: OnServerPropertyValueChanged);

	public static readonly BindableProperty IsRequiredProperty =
		BindableProperty.Create(
			nameof(IsRequired),
			typeof(bool),
			typeof(ServerPropertyInputView),
			false);

	public static readonly BindableProperty ModelProperty =
		BindableProperty.Create(
			nameof(Model),
			typeof(object),
			typeof(ServerPropertyInputView),
			propertyChanged: OnModelChanged);

	public PropertyDefinition? ServerPropertyDefinition
	{
		get => (PropertyDefinition?)GetValue(ServerPropertyDefinitionProperty);
		set => SetValue(ServerPropertyDefinitionProperty, value);
	}

	/// <summary>
	/// Optional explicit key, for example "motd".
	/// If omitted, the control tries to get the key from ServerPropertyDefinition.
	/// </summary>
	public string? ServerPropertyKey
	{
		get => (string?)GetValue(ServerPropertyKeyProperty);
		set => SetValue(ServerPropertyKeyProperty, value);
	}

	/// <summary>
	/// Path to the dictionary inside the model.
	/// If the model itself is Server, keep "Properties".
	/// If the model has Server.Properties, use "Server.Properties".
	/// </summary>
	public string PropertyDictionaryModelPath
	{
		get => (string)GetValue(PropertyDictionaryModelPathProperty);
		set => SetValue(PropertyDictionaryModelPathProperty, value);
	}

	public string? ServerPropertyValue
	{
		get => (string?)GetValue(ServerPropertyValueProperty);
		set => SetValue(ServerPropertyValueProperty, value);
	}

	public bool IsRequired
	{
		get => (bool)GetValue(IsRequiredProperty);
		set => SetValue(IsRequiredProperty, value);
	}

	public object? Model
	{
		get => GetValue(ModelProperty);
		set => SetValue(ModelProperty, value);
	}

	public string? ServerPropertyType => ServerPropertyDefinition?.Type.ToString();

	public List<KeyValuePair<string, string>>? Options =>
		ServerPropertyDefinition?.Options?.ToList();

	public string? ValueModelPath
	{
		get
		{
			if (ServerPropertyDefinition is null)
				return null;

			if (string.IsNullOrWhiteSpace(PropertyDictionaryModelPath))
				return null;

			return $"{PropertyDictionaryModelPath}[{ServerPropertyDefinition.Key}]";
		}
	}

	public int ListIndex
	{
		get
		{
			if (ServerPropertyValue is null || Options is null)
				return -1;

			return Options.FindIndex(kv => kv.Key == ServerPropertyValue);
		}

		set
		{
			if (Options is null || value < 0 || value >= Options.Count)
				return;

			ServerPropertyValue = Options[value].Key;
		}
	}

	public bool IsChecked
	{
		get
		{
			return string.Equals(ServerPropertyValue, "true", StringComparison.OrdinalIgnoreCase);
		}

		set
		{
			if (ServerPropertyType == "Boolean")
				ServerPropertyValue = value ? "true" : "false";
		}
	}

	public string? StringValue
	{
		get => ServerPropertyValue;

		set
		{
			if (ServerPropertyType == "String" ||
				ServerPropertyType == "Integer" ||
				ServerPropertyDefinition is null)
			{
				ServerPropertyValue = value;
			}
		}
	}

	public ServerPropertyInputView()
	{
		InitializeComponent();
	}

	public void SetModel(object? model)
	{
		Model = model;
	}

	public bool Validate()
	{
		if (!IsRequired)
			return true;

		return ServerPropertyType switch
		{
			"Boolean" => IsChecked,
			"Options" => ListIndex >= 0,
			"Integer" => !string.IsNullOrWhiteSpace(StringValue),
			"String" => !string.IsNullOrWhiteSpace(StringValue),
			_ => !string.IsNullOrWhiteSpace(ServerPropertyValue)
		};
	}

	public void SetInvalid()
	{

	}

	public void SetValid()
	{

	}

	private void LoadValueFromModel()
	{
		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(ValueModelPath))
			return;

		syncingFromModel = true;

		try
		{
			var value = ReflectionHelper.GetValue(Model, ValueModelPath);

			ServerPropertyValue = value?.ToString();
		}
		finally
		{
			syncingFromModel = false;
		}
	}

	private void WriteValueToModel()
	{
		if (syncingFromModel)
			return;

		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(ValueModelPath))
			return;

		ReflectionHelper.SetValue(Model, ValueModelPath, ServerPropertyValue);
	}

	private static void OnServerPropertyValueChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var view = (ServerPropertyInputView)bindable;

		view.WriteValueToModel();

		view.OnPropertyChanged(nameof(StringValue));
		view.OnPropertyChanged(nameof(IsChecked));
		view.OnPropertyChanged(nameof(ListIndex));
	}

	private static void OnServerPropertyDefinitionChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var view = (ServerPropertyInputView)bindable;

		view.OnPropertyChanged(nameof(ServerPropertyType));
		view.OnPropertyChanged(nameof(Options));
		view.OnPropertyChanged(nameof(ValueModelPath));
		view.OnPropertyChanged(nameof(StringValue));
		view.OnPropertyChanged(nameof(IsChecked));
		view.OnPropertyChanged(nameof(ListIndex));

		view.LoadValueFromModel();
	}

	private static void OnModelPathConfigChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var view = (ServerPropertyInputView)bindable;

		view.OnPropertyChanged(nameof(ValueModelPath));

		view.LoadValueFromModel();
	}

	private static void OnModelChanged(BindableObject bindable, object oldValue, object newValue)
	{
		((ServerPropertyInputView)bindable).LoadValueFromModel();
	}
}

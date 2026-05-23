namespace Wave.Ui.Views.FormComponents;

public partial class FormEntry : Entry, IFormElement
{
	private bool _syncingFromModel;

	public static readonly BindableProperty IsRequiredProperty =
		BindableProperty.Create(
			nameof(IsRequired),
			typeof(bool),
			typeof(FormEntry),
			false);

	public static readonly BindableProperty TextModelPathProperty =
		BindableProperty.Create(
			nameof(TextModelPath),
			typeof(string),
			typeof(FormEntry),
			default(string),
			propertyChanged: OnModelConfigChanged);

	public static readonly BindableProperty TextModelBindingModeProperty =
		BindableProperty.Create(
			nameof(TextModelBindingMode),
			typeof(BindingMode),
			typeof(FormEntry),
			BindingMode.TwoWay,
			propertyChanged: OnModelConfigChanged);

	public bool IsRequired
	{
		get => (bool)GetValue(IsRequiredProperty);
		set => SetValue(IsRequiredProperty, value);
	}

	public object? Model { get; private set; }

	public string? TextModelPath
	{
		get => (string?)GetValue(TextModelPathProperty);
		set => SetValue(TextModelPathProperty, value);
	}

	public BindingMode TextModelBindingMode
	{
		get => (BindingMode)GetValue(TextModelBindingModeProperty);
		set => SetValue(TextModelBindingModeProperty, value);
	}

	public void SetModel(object? model)
	{
		Model = model;
		LoadTextFromModel();
	}

	private static void OnModelConfigChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is FormEntry entry)
			entry.LoadTextFromModel();
	}

	protected override void OnPropertyChanged(string? propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == nameof(Text))
			WriteTextToModel();
	}

	private void LoadTextFromModel()
	{
		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(TextModelPath))
			return;

		if (!CanReadFromModel(TextModelBindingMode))
			return;

		_syncingFromModel = true;

		try
		{
			var value = ReflectionHelper.GetValue(Model, TextModelPath);
			Text = value?.ToString();
		}
		finally
		{
			_syncingFromModel = false;
		}
	}

	private void WriteTextToModel()
	{
		if (_syncingFromModel)
			return;

		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(TextModelPath))
			return;

		if (!CanWriteToModel(TextModelBindingMode))
			return;

		ReflectionHelper.SetValue(Model, TextModelPath, Text);
	}

	public bool Validate()
	{
		return !IsRequired || !string.IsNullOrWhiteSpace(Text);
	}

	public void SetInvalid()
	{
		// Add styling later.
	}

	public void SetValid()
	{
		// Add styling later.
	}

	private static bool CanReadFromModel(BindingMode mode)
	{
		return mode is BindingMode.OneWay
			or BindingMode.TwoWay
			or BindingMode.OneTime;
	}

	private static bool CanWriteToModel(BindingMode mode)
	{
		return mode is BindingMode.OneWayToSource
			or BindingMode.TwoWay;
	}
}
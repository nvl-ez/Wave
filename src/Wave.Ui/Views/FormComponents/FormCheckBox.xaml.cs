namespace Wave.Ui.Views.FormComponents;

using System;

public partial class FormCheckBox : CheckBox, IFormElement
{
	private bool _syncingFromModel;

	public static readonly BindableProperty IsRequiredProperty =
		BindableProperty.Create(
			nameof(IsRequired),
			typeof(bool),
			typeof(FormCheckBox),
			false);

	public static readonly BindableProperty IsCheckedModelPathProperty =
		BindableProperty.Create(
			nameof(IsCheckedModelPath),
			typeof(string),
			typeof(FormCheckBox),
			default(string),
			propertyChanged: OnModelConfigChanged);

	public static readonly BindableProperty IsCheckedModelBindingModeProperty =
		BindableProperty.Create(
			nameof(IsCheckedModelBindingMode),
			typeof(BindingMode),
			typeof(FormCheckBox),
			BindingMode.TwoWay,
			propertyChanged: OnModelConfigChanged);

	public bool IsRequired
	{
		get => (bool)GetValue(IsRequiredProperty);
		set => SetValue(IsRequiredProperty, value);
	}

	public object? Model { get; private set; }

	public string? IsCheckedModelPath
	{
		get => (string?)GetValue(IsCheckedModelPathProperty);
		set => SetValue(IsCheckedModelPathProperty, value);
	}

	public BindingMode IsCheckedModelBindingMode
	{
		get => (BindingMode)GetValue(IsCheckedModelBindingModeProperty);
		set => SetValue(IsCheckedModelBindingModeProperty, value);
	}

	public void SetModel(object? model)
	{
		Model = model;
		LoadIsCheckedFromModel();
	}

	private static void OnModelConfigChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is FormCheckBox checkBox)
			checkBox.LoadIsCheckedFromModel();
	}

	protected override void OnPropertyChanged(string? propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == nameof(IsChecked))
			WriteIsCheckedToModel();
	}

	private void LoadIsCheckedFromModel()
	{
		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(IsCheckedModelPath))
			return;

		if (!CanReadFromModel(IsCheckedModelBindingMode))
			return;

		_syncingFromModel = true;

		try
		{
			var value = ReflectionHelper.GetValue(Model, IsCheckedModelPath);

			IsChecked = value is bool boolValue && boolValue;
		}
		finally
		{
			_syncingFromModel = false;
		}
	}

	private void WriteIsCheckedToModel()
	{
		if (_syncingFromModel)
			return;

		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(IsCheckedModelPath))
			return;

		if (!CanWriteToModel(IsCheckedModelBindingMode))
			return;

		ReflectionHelper.SetValue(Model, IsCheckedModelPath, IsChecked);
	}

	public bool Validate()
	{
		// Required checkbox means it must be checked.
		return !IsRequired || IsChecked;
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
namespace Wave.Ui.Views.FormComponents;

using System.Collections;

public partial class FormPicker : Picker, IFormElement
{
	private bool syncingFromModel;

	public static readonly BindableProperty IsRequiredProperty =
		BindableProperty.Create(
			nameof(IsRequired),
			typeof(bool),
			typeof(FormPicker),
			false);

	public static readonly BindableProperty ItemsSourceModelPathProperty =
		BindableProperty.Create(
			nameof(ItemsSourceModelPath),
			typeof(string),
			typeof(FormPicker),
			default(string),
			propertyChanged: OnModelConfigChanged);

	public static readonly BindableProperty SelectedIndexModelPathProperty =
		BindableProperty.Create(
			nameof(SelectedIndexModelPath),
			typeof(string),
			typeof(FormPicker),
			default(string),
			propertyChanged: OnModelConfigChanged);

	public static readonly BindableProperty SelectedIndexModelBindingModeProperty =
		BindableProperty.Create(
			nameof(SelectedIndexModelBindingMode),
			typeof(BindingMode),
			typeof(FormPicker),
			BindingMode.TwoWay,
			propertyChanged: OnModelConfigChanged);

	public static readonly BindableProperty SelectedValueProperty =
		BindableProperty.Create(
			nameof(SelectedValue),
			typeof(object),
			typeof(FormPicker),
			default(object),
			BindingMode.TwoWay,
			propertyChanged: OnSelectedValueChanged);

	public static readonly BindableProperty SelectedValueModelPathProperty =
		BindableProperty.Create(
			nameof(SelectedValueModelPath),
			typeof(string),
			typeof(FormPicker),
			default(string),
			propertyChanged: OnModelConfigChanged);

	public static readonly BindableProperty SelectedValueModelBindingModeProperty =
		BindableProperty.Create(
			nameof(SelectedValueModelBindingMode),
			typeof(BindingMode),
			typeof(FormPicker),
			BindingMode.TwoWay,
			propertyChanged: OnModelConfigChanged);

	public static readonly BindableProperty SelectedValueMemberPathProperty =
		BindableProperty.Create(
			nameof(SelectedValueMemberPath),
			typeof(string),
			typeof(FormPicker),
			default(string),
			propertyChanged: OnSelectionConfigChanged);

	public static readonly BindableProperty DisplayMemberPathProperty =
		BindableProperty.Create(
			nameof(DisplayMemberPath),
			typeof(string),
			typeof(FormPicker),
			default(string),
			propertyChanged: OnDisplayConfigChanged);

	public static readonly BindableProperty DisplayMemberPathModelPathProperty =
		BindableProperty.Create(
			nameof(DisplayMemberPathModelPath),
			typeof(string),
			typeof(FormPicker),
			default(string),
			propertyChanged: OnModelConfigChanged);

	public bool IsRequired
	{
		get => (bool)GetValue(IsRequiredProperty);
		set => SetValue(IsRequiredProperty, value);
	}

	public object? Model { get; private set; }

	public string? ItemsSourceModelPath
	{
		get => (string?)GetValue(ItemsSourceModelPathProperty);
		set => SetValue(ItemsSourceModelPathProperty, value);
	}

	public string? SelectedIndexModelPath
	{
		get => (string?)GetValue(SelectedIndexModelPathProperty);
		set => SetValue(SelectedIndexModelPathProperty, value);
	}

	public BindingMode SelectedIndexModelBindingMode
	{
		get => (BindingMode)GetValue(SelectedIndexModelBindingModeProperty);
		set => SetValue(SelectedIndexModelBindingModeProperty, value);
	}

	public object? SelectedValue
	{
		get => GetValue(SelectedValueProperty);
		set => SetValue(SelectedValueProperty, value);
	}

	public string? SelectedValueModelPath
	{
		get => (string?)GetValue(SelectedValueModelPathProperty);
		set => SetValue(SelectedValueModelPathProperty, value);
	}

	public BindingMode SelectedValueModelBindingMode
	{
		get => (BindingMode)GetValue(SelectedValueModelBindingModeProperty);
		set => SetValue(SelectedValueModelBindingModeProperty, value);
	}

	public string? SelectedValueMemberPath
	{
		get => (string?)GetValue(SelectedValueMemberPathProperty);
		set => SetValue(SelectedValueMemberPathProperty, value);
	}

	public string? DisplayMemberPath
	{
		get => (string?)GetValue(DisplayMemberPathProperty);
		set => SetValue(DisplayMemberPathProperty, value);
	}

	public string? DisplayMemberPathModelPath
	{
		get => (string?)GetValue(DisplayMemberPathModelPathProperty);
		set => SetValue(DisplayMemberPathModelPathProperty, value);
	}

	public FormPicker()
	{
		SelectedIndexChanged += OnSelectedIndexChanged;
	}

	public void SetModel(object? model)
	{
		Model = model;

		LoadItemsSourceFromModel();
		LoadDisplayMemberPathFromModel();
		ApplyDisplayMemberPath();
		LoadSelectionFromModel();
	}

	private static void OnModelConfigChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not FormPicker picker)
			return;

		picker.LoadItemsSourceFromModel();
		picker.LoadDisplayMemberPathFromModel();
		picker.ApplyDisplayMemberPath();
		picker.LoadSelectionFromModel();
	}

	private static void OnSelectionConfigChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not FormPicker picker)
			return;

		picker.UpdateSelectedValueFromSelectedItem();
		picker.WriteSelectionToModel();
	}

	private static void OnDisplayConfigChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is FormPicker picker)
			picker.ApplyDisplayMemberPath();
	}

	private static void OnSelectedValueChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not FormPicker picker)
			return;

		if (picker.syncingFromModel)
			return;

		picker.SelectItemByValue(newValue);
		picker.WriteSelectionToModel();
	}

	protected override void OnPropertyChanged(string? propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == nameof(ItemsSource))
			LoadSelectionFromModel();

		if (propertyName == nameof(SelectedItem))
		{
			UpdateSelectedValueFromSelectedItem();
			WriteSelectionToModel();
		}
	}

	private void OnSelectedIndexChanged(object? sender, EventArgs e)
	{
		UpdateSelectedValueFromSelectedItem();
		WriteSelectionToModel();
	}

	private void LoadItemsSourceFromModel()
	{
		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(ItemsSourceModelPath))
			return;

		var value = ReflectionHelper.GetValue(Model, ItemsSourceModelPath);

		if (value is IList list)
			ItemsSource = list;
	}

	private void LoadDisplayMemberPathFromModel()
	{
		if (Model is null)
			return;

		if (string.IsNullOrWhiteSpace(DisplayMemberPathModelPath))
			return;

		var value = ReflectionHelper.GetValue(Model, DisplayMemberPathModelPath);

		if (value is string path)
			DisplayMemberPath = path;
	}

	private void ApplyDisplayMemberPath()
	{
		if (string.IsNullOrWhiteSpace(DisplayMemberPath))
			return;

		ItemDisplayBinding = new Binding(DisplayMemberPath);
	}

	private void LoadSelectionFromModel()
	{
		if (Model is null)
			return;

		syncingFromModel = true;

		try
		{
			if (CanReadFromModel(SelectedIndexModelBindingMode) &&
				!string.IsNullOrWhiteSpace(SelectedIndexModelPath))
			{
				var value = ReflectionHelper.GetValue(Model, SelectedIndexModelPath);

				if (value is int index)
					SelectedIndex = index;
			}

			if (CanReadFromModel(SelectedValueModelBindingMode) &&
				!string.IsNullOrWhiteSpace(SelectedValueModelPath))
			{
				var value = ReflectionHelper.GetValue(Model, SelectedValueModelPath);

				SelectedValue = value;
				SelectItemByValue(value);
			}
		}
		finally
		{
			syncingFromModel = false;
		}
	}

	private void WriteSelectionToModel()
	{
		if (syncingFromModel)
			return;

		if (Model is null)
			return;

		if (CanWriteToModel(SelectedIndexModelBindingMode) &&
			!string.IsNullOrWhiteSpace(SelectedIndexModelPath))
		{
			ReflectionHelper.SetValue(Model, SelectedIndexModelPath, SelectedIndex);
		}

		if (CanWriteToModel(SelectedValueModelBindingMode) &&
			!string.IsNullOrWhiteSpace(SelectedValueModelPath))
		{
			ReflectionHelper.SetValue(Model, SelectedValueModelPath, SelectedValue);
		}
	}

	private void UpdateSelectedValueFromSelectedItem()
	{
		if (SelectedItem is null)
		{
			SelectedValue = null;
			return;
		}

		if (string.IsNullOrWhiteSpace(SelectedValueMemberPath))
		{
			SelectedValue = SelectedItem;
			return;
		}

		SelectedValue = ReflectionHelper.GetValue(SelectedItem, SelectedValueMemberPath);
	}

	private void SelectItemByValue(object? value)
	{
		if (ItemsSource is null)
			return;

		for (var i = 0; i < ItemsSource.Count; i++)
		{
			var item = ItemsSource[i];

			object? itemValue = string.IsNullOrWhiteSpace(SelectedValueMemberPath)
				? item
				: ReflectionHelper.GetValue(item, SelectedValueMemberPath);

			if (Equals(itemValue, value))
			{
				SelectedIndex = i;
				SelectedItem = item;
				return;
			}
		}

		SelectedIndex = -1;
		SelectedItem = null;
	}

	public bool Validate()
	{
		return !IsRequired || SelectedIndex >= 0;
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
namespace Wave.Ui.Views.Navigation;

public class KeepAliveTabHost : Grid
{
	public static readonly BindableProperty CurrentTabProperty =
		BindableProperty.Create(
			nameof(CurrentTab),
			typeof(string),
			typeof(KeepAliveTabHost),
			default(string),
			propertyChanged: OnCurrentTabChanged);

	public string? CurrentTab
	{
		get => (string?)GetValue(CurrentTabProperty);
		set => SetValue(CurrentTabProperty, value);
	}

	public static readonly BindableProperty TabKeyProperty =
		BindableProperty.CreateAttached(
			"TabKey",
			typeof(string),
			typeof(KeepAliveTabHost),
			default(string),
			propertyChanged: OnTabKeyChanged);

	public static string? GetTabKey(BindableObject view)
	{
		return (string?)view.GetValue(TabKeyProperty);
	}

	public static void SetTabKey(BindableObject view, string? value)
	{
		view.SetValue(TabKeyProperty, value);
	}

	protected override void OnChildAdded(Element child)
	{
		base.OnChildAdded(child);

		if (child is View view)
			UpdateChildVisibility(view);
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		UpdateAllChildren();
	}

	private static void OnCurrentTabChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is KeepAliveTabHost host)
			host.UpdateAllChildren();
	}

	private static void OnTabKeyChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view && view.Parent is KeepAliveTabHost host)
			host.UpdateChildVisibility(view);
	}

	private void UpdateAllChildren()
	{
		foreach (var child in Children.OfType<View>())
			UpdateChildVisibility(child);
	}

	private void UpdateChildVisibility(View child)
	{
		var tabKey = GetTabKey(child);

		child.IsVisible = string.Equals(
			tabKey,
			CurrentTab,
			StringComparison.Ordinal);
	}
}
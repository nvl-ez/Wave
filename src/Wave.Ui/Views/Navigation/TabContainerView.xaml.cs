using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Wave.Ui.Views.Navigation;

[ContentProperty(nameof(Tabs))]
public partial class TabContainerView : ContentView
{
	public static readonly BindableProperty OrientationProperty =
		BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(TabContainerView), StackOrientation.Horizontal, propertyChanged: OnOrientationChanged);

	public StackOrientation Orientation
	{
		get => (StackOrientation)GetValue(OrientationProperty);
		set { SetValue(OrientationProperty, value); }
	}

	public ObservableCollection<TabView> Tabs { get; } = new();

	public TabContainerView()
	{
		InitializeComponent();
		RootLayout.Orientation = Orientation;
		Tabs.CollectionChanged += OnCollectionChanged;
	}

	private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
		{
			foreach (TabView tab in e.NewItems)
				RootLayout.Children.Add(tab);
		}

		if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
		{
			foreach (TabView tab in e.OldItems)
				RootLayout.Children.Remove(tab);
		}

		if (e.Action == NotifyCollectionChangedAction.Reset)
		{
			RootLayout.Children.Clear();
		}
	}

	private static void OnOrientationChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (TabContainerView)bindable;
		control.RootLayout.Orientation = (StackOrientation)newValue;
	}

}
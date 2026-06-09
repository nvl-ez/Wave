using System.Collections;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Wave.Domain.Utils;
using Wave.Ui.Utils;
using System.Threading.Tasks;
namespace Wave.Ui.Views.CollectionComponents;

public partial class PaginatedCollectionView : ContentView
{
	private bool IsChanging = false;
	//Property Deffinitions
	public static readonly BindableProperty ItemsSourceProperty =
		BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PaginatedCollectionView), default(IEnumerable));
	public static readonly BindableProperty ItemTemplateProperty =
		BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(PaginatedCollectionView), default(DataTemplate));
	public static readonly BindableProperty ItemsLayoutProperty =
		BindableProperty.Create(nameof(ItemsLayout), typeof(ItemsLayout), typeof(PaginatedCollectionView), default(ItemsLayout));
	public static readonly BindableProperty SelectionModeProperty =
		BindableProperty.Create(nameof(SelectionMode), typeof(SelectionMode), typeof(PaginatedCollectionView), SelectionMode.None);
	public static readonly BindableProperty PaginationStateProperty =
		BindableProperty.Create(nameof(PaginationState), typeof(ObservablePaginationState), typeof(PaginatedCollectionView), default(ObservablePaginationState), propertyChanged: OnPaginationStateChanged, defaultBindingMode: BindingMode.TwoWay);
	public static readonly BindableProperty NavigateCommandProperty =
		BindableProperty.Create(nameof(NavigateCommand), typeof(IAsyncRelayCommand), typeof(PaginatedCollectionView), default(IAsyncRelayCommand));
	public static readonly BindableProperty SelectionChangedCommandProperty =
		BindableProperty.Create(nameof(SelectionChangedCommand), typeof(IAsyncRelayCommand), typeof(PaginatedCollectionView), default(IAsyncRelayCommand));
	public static readonly BindableProperty SelectionChangedCommandParameterProperty =
		BindableProperty.Create(nameof(SelectionChangedCommandParameter), typeof(object), typeof(PaginatedCollectionView), default);
	public static readonly BindableProperty SelectedItemProperty =
		BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(PaginatedCollectionView), default, defaultBindingMode: BindingMode.TwoWay);
	public IEnumerable ItemsSource
	{
		get => (IEnumerable)GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}
	public DataTemplate ItemTemplate
	{
		get => (DataTemplate)GetValue(ItemTemplateProperty);
		set => SetValue(ItemTemplateProperty, value);
	}
	public ItemsLayout ItemsLayout
	{
		get => (ItemsLayout)GetValue(ItemsLayoutProperty);
		set => SetValue(ItemsLayoutProperty, value);
	}
	public SelectionMode SelectionMode
	{
		get => (SelectionMode)GetValue(SelectionModeProperty);
		set => SetValue(SelectionModeProperty, value);
	}
	public ObservablePaginationState PaginationState
	{
		get => (ObservablePaginationState)GetValue(PaginationStateProperty);
		set => SetValue(PaginationStateProperty, value);
	}
	public IAsyncRelayCommand NavigateCommand
	{
		get => (IAsyncRelayCommand)GetValue(NavigateCommandProperty);
		set => SetValue(NavigateCommandProperty, value);
	}
	public IAsyncRelayCommand SelectionChangedCommand
	{
		get => (IAsyncRelayCommand)GetValue(SelectionChangedCommandProperty);
		set => SetValue(SelectionChangedCommandProperty, value);
	}
	public object? SelectionChangedCommandParameter
	{
		get => GetValue(SelectionChangedCommandParameterProperty);
		set => SetValue(SelectionChangedCommandParameterProperty, value);
	}
	public object? SelectedItem
	{
		get => GetValue(SelectedItemProperty);
		set => SetValue(SelectedItemProperty, value);
	}
	//State Deffinitions
	public bool IsBeginningVisible
	{
		get
		{
			if (PaginationState is null) return false;
			return PaginationState.Index > 0;
		}
	}
	public bool IsPreviousVisible
	{
		get
		{
			if (PaginationState is null || PaginationState.Index < 0) return false;
			return PaginationState.Index > 0;
		}
	}
	public bool IsNextVisible
	{
		get
		{
			if (PaginationState is null || PaginationState.Index < 0) return false;
			return PaginationState.Index < PaginationState.TotalCount - PaginationState.PageSize;
		}
	}
	public bool IsLastVisible
	{
		get
		{
			if (PaginationState is null || PaginationState.Index < 0) return false;
			return PaginationState.Index < PaginationState.TotalCount - PaginationState.PageSize;
		}
	}
	public string PageLabel
	{
		get
		{
			if (PaginationState is null || PaginationState.Index < 0 || PaginationState.ResultCount <= 0) return "-/-";
			int current = PaginationState.Index / PaginationState.PageSize + 1;
			int total = (int)Math.Ceiling((double)PaginationState.TotalCount / PaginationState.PageSize);
			return $"{current}/{total}";
		}
	}

	public PaginatedCollectionView()
	{
		InitializeComponent();
	}

	// Updates from the ObservablePaginationState
	private static void OnPaginationStateChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (PaginatedCollectionView)bindable;

		if (oldValue is ObservablePaginationState oldState)
			oldState.PropertyChanged -= control.PaginationState_PropertyChanged;

		if (newValue is ObservablePaginationState newState)
			newState.PropertyChanged += control.PaginationState_PropertyChanged;
	}

	private void PaginationState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (
			e.PropertyName == nameof(ObservablePaginationState.Index) ||
			e.PropertyName == nameof(ObservablePaginationState.PageSize) ||
			e.PropertyName == nameof(ObservablePaginationState.ResultCount) ||
			e.PropertyName == nameof(ObservablePaginationState.TotalCount)
		)
		{
			OnPropertyChanged(nameof(IsBeginningVisible));
			OnPropertyChanged(nameof(IsPreviousVisible));
			OnPropertyChanged(nameof(IsNextVisible));
			OnPropertyChanged(nameof(IsLastVisible));
			OnPropertyChanged(nameof(PageLabel));
		}
	}

	//Commands
	private async void Beginning(object sender, EventArgs e)
	{
		if (PaginationState is null || PaginationState.Index < 0) return;
		PaginationState.Index = 0;

		await Navigate();
	}

	private async void Previous(object sender, EventArgs e)
	{
		if (PaginationState is null || PaginationState.Index < 0) return;
		int nextIndex = PaginationState.Index - PaginationState.PageSize;
		PaginationState.Index = Math.Max(0, nextIndex);

		await Navigate();
	}

	private async void Next(object sender, EventArgs e)
	{
		if (PaginationState is null || PaginationState.Index < 0) return;
		int nextIndex = PaginationState.Index + PaginationState.PageSize;
		PaginationState.Index = Math.Min(nextIndex, PaginationState.TotalCount);

		await Navigate();
	}

	private async void End(object sender, EventArgs e)
	{
		if (PaginationState is null || PaginationState.Index < 0) return;
		PaginationState.Index = PaginationState.TotalCount - PaginationState.PageSize;

		await Navigate();
	}

	private async Task Navigate()
	{
		if (IsChanging) return;

		try
		{
			IsChanging = true;

			if (NavigateCommand?.CanExecute(null) == true)
				await NavigateCommand.ExecuteAsync(null);
		}
		finally
		{
			IsChanging = false;
		}
	}

}
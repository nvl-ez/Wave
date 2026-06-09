using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Mvvm.Input;

namespace Wave.Ui.Views.FormComponents;

public partial class Form : ContentView
{
	public static readonly BindableProperty ModelProperty =
		BindableProperty.Create(
			nameof(Model),
			typeof(object),
			typeof(Form),
			defaultValue: null,
			propertyChanged: OnModelChanged);

	public static readonly BindableProperty SubmitCommandProperty =
		BindableProperty.Create(
			nameof(SubmitCommand),
			typeof(IAsyncRelayCommand),
			typeof(Form));

	public object? Model
	{
		get => GetValue(ModelProperty);
		set => SetValue(ModelProperty, value);
	}

	public IAsyncRelayCommand? SubmitCommand
	{
		get => (IAsyncRelayCommand?)GetValue(SubmitCommandProperty);
		set => SetValue(SubmitCommandProperty, value);
	}

	public Form()
	{
		InitializeComponent();

		ChildAdded += OnChildAdded;
	}

	private static void OnModelChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Form form)
			form.AssignModelToChildren();
	}

	protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == nameof(Content))
			AssignModelToChildren();
	}

	private void OnChildAdded(object? sender, ElementEventArgs e)
	{
		AssignModel(e.Element);
	}

	private void AssignModel(Element element)
	{
		var visited = new HashSet<Element>();

		foreach (var formElement in GetFormElements(element, visited))
		{
			if (formElement is not SubmitButton)
				formElement.SetModel(Model);
		}
	}

	public void AssignModelToChildren()
	{
		if (Content is not Element content)
			return;

		var visited = new HashSet<Element>();

		foreach (var formElement in GetFormElements(content, visited))
		{
			if (formElement is not SubmitButton)
				formElement.SetModel(Model);
		}
	}

	public bool Validate()
	{
		var valid = true;

		if (Content is not Element content)
			return valid;

		var visited = new HashSet<Element>();

		foreach (var formElement in GetFormElements(content, visited))
		{
			if (formElement is SubmitButton)
				continue;

			if (!formElement.Validate())
			{
				formElement.SetInvalid();
				valid = false;
			}
			else
			{
				formElement.SetValid();
			}
		}

		return valid;
	}

	public void Submit()
	{
		if (!Validate())
			return;

		if (SubmitCommand?.CanExecute(Model) == true)
			SubmitCommand.Execute(Model);
	}

	private IEnumerable<IFormElement> GetFormElements(Element root, HashSet<Element> visited)
	{
		foreach (var element in GetReachableElements(root, visited))
		{
			if (element is IFormElement formElement)
				yield return formElement;
		}
	}

	private IEnumerable<Element> GetReachableElements(Element root, HashSet<Element> visited)
	{
		foreach (var element in GetVisualElements(root))
		{
			if (!visited.Add(element))
				continue;

			if (IsInsideNestedForm(element))
				continue;

			yield return element;

			if (element is not BindableObject bindable)
				continue;

			var stateViews = StateContainer.GetStateViews(bindable);

			foreach (var stateView in stateViews.OfType<Element>())
			{
				foreach (var stateElement in GetReachableElements(stateView, visited))
					yield return stateElement;
			}
		}
	}

	private static IEnumerable<Element> GetVisualElements(Element root)
	{
		yield return root;

		foreach (var descendant in root.GetVisualTreeDescendants().OfType<Element>())
			yield return descendant;
	}

	private bool IsInsideNestedForm(Element element)
	{
		if (ReferenceEquals(element, this))
			return false;

		if (element is Form)
			return true;

		var parent = element.Parent;

		while (parent is not null)
		{
			if (ReferenceEquals(parent, this))
				return false;

			if (parent is Form)
				return true;

			parent = parent.Parent;
		}

		return false;
	}
}
using System.Windows.Input;
using CommunityToolkit.Maui.Layouts;

namespace Wave.Ui.Views.FormComponents;

public partial class Form : VerticalStackLayout
{
	//Property for the model
	public static readonly BindableProperty ModelProperty = BindableProperty.Create(nameof(Model), typeof(object), typeof(Form), defaultValue: null, propertyChanged: OnModelChanged);
	//Property for the function
	public static readonly BindableProperty SubmitCommandProperty = BindableProperty.Create(nameof(SubmitCommand), typeof(ICommand), typeof(Form));

	public object? Model { get => GetValue(ModelProperty); set => SetValue(ModelProperty, value); }
	public ICommand? SubmitCommand { get => (ICommand?)GetValue(SubmitCommandProperty); set => SetValue(SubmitCommandProperty, value); }

	public Form()
	{
		InitializeComponent();
	}

	private static void OnModelChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Form form) form.AssignModelToChildren();
	}

	private void OnChildAdded(object sender, ElementEventArgs e)
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

	private IEnumerable<IFormElement> GetFormElements(Element element, HashSet<Element> visited)
	{
		if (!visited.Add(element))
			yield break;

		if (element is IFormElement formElement)
			yield return formElement;

		foreach (var child in element.GetVisualTreeDescendants().OfType<Element>())
		{
			foreach (var result in GetFormElements(child, visited))
				yield return result;
		}

		if (element is BindableObject bindable)
		{
			var stateViews = StateContainer.GetStateViews(bindable);

			foreach (var stateView in stateViews.OfType<Element>())
			{
				foreach (var result in GetFormElements(stateView, visited))
					yield return result;
			}

		}
	}

	public void AssignModelToChildren()
	{
		var visited = new HashSet<Element>();

		foreach (var child in Children.OfType<Element>())
		{
			foreach (var formElement in GetFormElements(child, visited))
			{
				if (formElement is not SubmitButton)
					formElement.SetModel(Model);
			}
		}
	}

	public bool Validate()
	{
		var valid = true;
		var visited = new HashSet<Element>();

		foreach (var child in Children.OfType<Element>())
		{
			foreach (var formElement in GetFormElements(child, visited))
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
		}

		return valid;
	}

	public void Submit()
	{
		if (!Validate()) return;

		if (SubmitCommand?.CanExecute(Model) == true)
			SubmitCommand.Execute(Model);
	}
}
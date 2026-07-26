namespace Wave.Ui;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new Window(new AppShell());
        ConfigureWindow(window);
        window.Destroying += WindowDestroying;
        return window;
    }

    partial void ConfigureWindow(Window window);

    private async void WindowDestroying(object? sender, EventArgs e)
    {
        await AppComposition.GetServerExecutorService().StopAll();
    }
}

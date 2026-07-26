using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;

namespace Wave.Ui;

public partial class App
{
    private bool closing;

    partial void ConfigureWindow(Window window)
    {
        window.Created += WindowCreated;
    }

    private void WindowCreated(object? sender, EventArgs e)
    {
        if (sender is not Window window || window.Handler?.PlatformView is not MauiWinUIWindow platformWindow) return;
        platformWindow.AppWindow.Closing += AppWindowClosing;
    }

    private async void AppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        if (closing) return;

        args.Cancel = true;
        closing = true;

        bool confirmed = await Windows[0].Page!.DisplayAlertAsync(
            "Close Wave?",
            "Are you sure you want to close the app? Closing Wave will stop all running servers and could cause unsaved progress to be lost.",
            "Close",
            "Cancel");

        if (!confirmed)
        {
            closing = false;
            return;
        }

        await AppComposition.GetServerExecutorService().StopAll();
        sender.Destroy();
    }
}

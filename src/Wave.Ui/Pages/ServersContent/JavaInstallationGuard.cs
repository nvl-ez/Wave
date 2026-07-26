using Wave.Application.Out.Java;

namespace Wave.Ui.Pages.ServersContent;

internal static class JavaInstallationGuard
{
    public static async Task<bool> CanContinueAsync(
        IJavaInstallRepository javaInstallRepository,
        CancellationToken ct = default)
    {
        if ((await javaInstallRepository.GetAllAsync(ct)).Any())
        {
            return true;
        }

        bool goToSettings = await Shell.Current.DisplayAlertAsync(
            "Java installation required",
            "This action cannot be completed because no Java version is installed.",
            "Go to Settings",
            "Cancel");

        if (goToSettings)
        {
            await Shell.Current.GoToAsync("//settings");
        }

        return false;
    }
}

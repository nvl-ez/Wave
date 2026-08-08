namespace Wave.Domain.Configuration;

using Wave.Domain.Java;

public class ApplicationConfiguration
{
    public string? CurseforgeApiToken { get; set; }
    public JavaInstallation? JavaInstallation { get; set; }
}

using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Properties;

namespace Wave.Ui.Pages.ServerContent.Classes;

public record class ServerPropertyQuery
{
    public required ServerQuery Server { get; init; }
    public required PropertyDefinition Definition { get; init; }
}

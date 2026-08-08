using Wave.Domain.Mods;

namespace Wave.Domain.ServerManager;

public class ModMigrationResult
{
    public IEnumerable<ModFile> DeletedMods { get; set; } = [];

    public IEnumerable<ModFile> FailedMods { get; set; } = [];

    public IEnumerable<ModFile> IncompatibleMods { get; set; } = [];

    public IEnumerable<ModFile> RequiredMods { get; set; } = [];
}

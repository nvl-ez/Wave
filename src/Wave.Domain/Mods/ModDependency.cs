using System;

namespace Wave.Domain.Mods;

public class ModDependency
{
    public required ModDependencyType DependencyType { get; set; }
    public required string ModId { get; set; }
    public string? VersionId { get; set; }

}

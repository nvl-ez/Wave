using System;

namespace Wave.Domain.ServerManager.Properties;

public record class PropertyDefinition
{
    public required string DisplayName { get; set; }
    public required string Key { get; set; }
    public required PropertyType Type { get; set; }
    public Dictionary<string, string>? Options { get; set; } = null;
}

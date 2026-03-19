using System;

namespace Wave.Domain.Minecraft;

public record class ServerPropertyDefinition
{
    public required string DisplayName { get; set; }
    public required string Key { get; set; }
    public required ServerPropertyType Type { get; set; }
    public Dictionary<string, string>? Options { get; set; } = null;
}

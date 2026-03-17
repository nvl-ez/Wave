using System;

namespace Wave.Domain.Minecraft;

public class ServerPropertyDefinition
{
    public required string DisplayName { get; set; }
    public required string Key { get; set; }
    public required ServerPropertyType Type { get; set; }
    public Dictionary<string, string>? Values { get; set; } = null;
}

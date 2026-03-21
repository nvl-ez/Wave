using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Minecraft.Versions.VersionDetailDtos;

public record class JavaVersionDto
{
    [JsonPropertyName("majorVersion")]
    public int MajorVersion { get; set; }

    [JsonPropertyName("component")]
    public string Component { get; set; } = string.Empty;
}

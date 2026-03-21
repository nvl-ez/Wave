using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Minecraft.Versions.VersionDetailDtos;

public record class VersionDetailDto
{
    [JsonPropertyName("downloads")]
    public required DownloadsDto Downloads { get; set; }

    [JsonPropertyName("javaVersion")]
    public required JavaVersionDto JavaVersion { get; set; }
}

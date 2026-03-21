using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Minecraft.Versions.VersionDetailDtos;

public record class DownloadsDto
{
    [JsonPropertyName("server")]
    public required ServerDto Server { get; set; }
}

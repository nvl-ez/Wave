using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Minecraft.Versions.VersionDetailDtos;

public class ServerDto
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}

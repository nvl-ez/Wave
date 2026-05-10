using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Modloader.Fabric.Api.Dtos;

public class InstallerInfoDto
{
    [JsonPropertyName("url")]
    public required string DownloadUrl { get; set; }
    [JsonPropertyName("maven")]
    public required string Maven { get; set; }
    [JsonPropertyName("version")]
    public required string Version { get; set; }
    [JsonPropertyName("stable")]
    public required bool Stable { get; set; }
}

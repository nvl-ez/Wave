using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Installer.ManifestDtos;

public record class DownloadDto
{
    [JsonPropertyName("sha1")]
    public string? Sha1 { get; set; }

    [JsonPropertyName("size")]
    public long? Size { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

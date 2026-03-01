using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Adoptium.Dtos;

public record class LatestAssetDto
{
    [JsonPropertyName("binary")]
    public required BinaryDto Binary { get; set; }
    [JsonPropertyName("release_name")]
    public required string ReleaseName { get; set; }
    [JsonPropertyName("release_link")]
    public required string ReleaseLink { get; set; }
    [JsonPropertyName("version")]
    public required VersionDto Version { get; set; }
}

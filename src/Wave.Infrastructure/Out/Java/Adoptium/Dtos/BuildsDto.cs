using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Adoptium.Dtos;

public record class BuildsDto
{
    [JsonPropertyName("binaries")]
    public required List<BinaryDto> Binaries { get; set; }
    [JsonPropertyName("release_name")]
    public required string ReleaseName { get; set; }
    [JsonPropertyName("release_link")]
    public required string ReleaseLink { get; set; }
    [JsonPropertyName("version_data")]
    public required VersionDto Version { get; set; }
}

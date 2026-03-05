using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Mojang.Dtos;

public record class ReleaseDto
{
    [JsonPropertyName("manifest")]
    public required ManifestDto Manifest { get; set; }
    [JsonPropertyName("version")]
    public required VersionDto Version { get; set; }
}

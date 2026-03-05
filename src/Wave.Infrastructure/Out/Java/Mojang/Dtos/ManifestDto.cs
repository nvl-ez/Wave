using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Mojang.Dtos;

public record class ManifestDto
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}

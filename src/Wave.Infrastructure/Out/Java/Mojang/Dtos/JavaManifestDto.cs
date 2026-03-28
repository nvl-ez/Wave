using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Mojang.Dtos;

public record class JavaManifestDto
{
    [JsonPropertyName("files")]
    public Dictionary<string, EntryDto>? Files { get; set; }
}

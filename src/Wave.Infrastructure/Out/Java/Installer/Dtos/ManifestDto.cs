using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Installer.Dtos;

public record class ManifestDto
{
    [JsonPropertyName("files")]
    public Dictionary<string, EntryDto>? Files { get; set; }
}

using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Mojang.Dtos;

public record class EntryDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("downloads")]
    public DownloadsDto? Downloads { get; set; }

    [JsonPropertyName("executable")]
    public bool? Executable { get; set; }

    [JsonPropertyName("target")]
    public string? Target { get; set; }
}

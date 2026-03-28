using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Mojang.Dtos;

public record class DownloadsDto
{
    [JsonPropertyName("raw")]
    public DownloadDto? Raw { get; set; }

    [JsonPropertyName("lzma")]
    public DownloadDto? Lzma { get; set; }
}

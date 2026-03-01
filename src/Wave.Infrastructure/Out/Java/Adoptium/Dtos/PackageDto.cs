using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Adoptium.Dtos;

public record class PackageDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("link")]
    public required string DownloadUrl { get; set; }
}
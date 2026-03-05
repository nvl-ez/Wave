using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Mojang.Dtos;

public record class VersionDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("released")]
    public required DateTime ReleaseDate { get; set; }
}

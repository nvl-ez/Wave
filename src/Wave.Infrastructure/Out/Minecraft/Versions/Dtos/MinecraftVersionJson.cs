using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Minecraft.Versions.Dtos;

public record class MinecraftVersionJson
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    [JsonPropertyName("url")]
    public required string Url { get; init; }
    [JsonPropertyName("time")]
    public required DateTime Time { get; init; }
    [JsonPropertyName("releaseTime")]
    public required DateTime ReleaseTime { get; init; }
}

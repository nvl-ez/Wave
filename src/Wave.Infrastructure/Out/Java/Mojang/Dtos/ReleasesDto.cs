using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Mojang.Dtos;

public record class ReleasesDto
{
    [JsonPropertyName("")]
    public Dictionary<string, Dictionary<string, List<ReleaseDto>>>? Platforms { get; set; }
}

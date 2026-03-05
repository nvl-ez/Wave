using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Adoptium.Dtos;

public record class VersionDto
{
    [JsonPropertyName("major")]
    public required int Major { get; set; }
    [JsonPropertyName("minor")]
    public required int Minor { get; set; }
    [JsonPropertyName("security")]
    public int? Security { get; set; }
    [JsonPropertyName("patch")]
    public int? Patch { get; set; }
    [JsonPropertyName("pre")]
    public string? Pre { get; set; }
    [JsonPropertyName("adopt_build_number")]
    public int? AdoptBuildNumber { get; set; }
    [JsonPropertyName("semver")]
    public required string Semver { get; set; }
    [JsonPropertyName("openjdk_version")]
    public required string OpenJdkVersion { get; set; }
    [JsonPropertyName("build")]
    public required int Build { get; set; }
    [JsonPropertyName("optional")]
    public string? Optional { get; set; }
}

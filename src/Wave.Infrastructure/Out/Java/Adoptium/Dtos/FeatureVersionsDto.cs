using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Adoptium.Dtos;

public record class FeatureVersionsDto
{
    [JsonPropertyName("available_lts_releases")]
    public required List<int> AvailableLtsReleases { get; set; }
    [JsonPropertyName("available_releases")]
    public required List<int> AvailableReleases { get; set; }
    [JsonPropertyName("most_recent_feature_release")]
    public required int MostRecentFeatureRelease { get; set; }
    [JsonPropertyName("most_recent_feature_version")]
    public required int MostRecentFeatureVersion { get; set; }
    [JsonPropertyName("most_recent_lts")]
    public required int MostRecentLts { get; set; }
}

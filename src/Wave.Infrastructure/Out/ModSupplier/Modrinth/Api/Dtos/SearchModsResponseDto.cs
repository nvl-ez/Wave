using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public record class SearchModsResponseDto
{
    [JsonPropertyName("hits")]
    public List<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
    [JsonPropertyName("offset")]
    public int Offset { get; set; } = -1;
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 0;
    [JsonPropertyName("total_hits")]
    public int TotalHits { get; set; } = 0;
}

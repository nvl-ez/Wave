using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public record class ProjectDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
    [JsonPropertyName("slug")]
    public string Slug { get; set; } = "";
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
    [JsonPropertyName("project_id")]
    public required string ProjectId { get; set; }
    [JsonPropertyName("author")]
    public required string Author { get; set; }
}

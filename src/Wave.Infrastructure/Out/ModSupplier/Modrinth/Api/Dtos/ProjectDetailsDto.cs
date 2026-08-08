using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public class ProjectDetailsDto
{
    [JsonPropertyName("id")]
    public string ProjectId { get; set; } = "";
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";
    [JsonPropertyName("slug")]
    public string Slug { get; set; } = "";
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }
    [JsonPropertyName("body")]
    public string Body { get; set; } = "";
}

using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public record class ProjectVersionDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("id")]
    public required string FileId { get; set; }
    [JsonPropertyName("project_id")]
    public required string ProjectId { get; set; }
    [JsonPropertyName("dependencies")]
    public required List<ModDependencyDto> Dependencies { get; set; }
    [JsonPropertyName("files")]
    public required List<FileDto> Files { get; set; }
}

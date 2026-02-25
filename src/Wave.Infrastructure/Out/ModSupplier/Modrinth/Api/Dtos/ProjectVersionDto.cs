using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public record class ProjectVersionDto
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("id")]
    public required string VersionId { get; set; }
    [JsonPropertyName("project_id")]
    public required string ProjectId { get; set; }
    [JsonPropertyName("dependencies")]
    public required List<ModDependencyDto> Dependencies { get; set; }
    [JsonPropertyName("files")]
    public required List<FileDto> Files { get; set; }
    [JsonPropertyName("version_number")]
    public required string Version { get; set; }
    [JsonPropertyName("changelog")]
    public required string Changelog { get; set; }
    [JsonPropertyName("version_type")]
    public required string VersionType { get; set; }
    [JsonPropertyName("featured")]
    public required bool Featured { get; set; }
}

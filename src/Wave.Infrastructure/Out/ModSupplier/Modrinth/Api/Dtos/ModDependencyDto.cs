using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public record class ModDependencyDto
{
    [JsonPropertyName("project_id")]
    public required string ProjectId { get; set; }
    [JsonPropertyName("version_id")]
    public required string VersionId { get; set; }
    [JsonPropertyName("dependency_type")]
    public required string DependencyType { get; set; }
}

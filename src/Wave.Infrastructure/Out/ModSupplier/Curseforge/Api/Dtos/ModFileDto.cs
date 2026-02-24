using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public record class ModFileDto
{
    [JsonPropertyName("id")]
    public required int FileId { get; set; }
    [JsonPropertyName("modId")]
    public required int ModId { get; set; }
    [JsonPropertyName("displayName")]
    public required string DisplayName { get; set; }
    [JsonPropertyName("downloadUrl")]
    public required string DownloadUrl { get; set; }
    [JsonPropertyName("dependencies")]
    public required List<ModDependencyDto> Dependencies { get; set; }
}

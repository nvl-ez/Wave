using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public record class ModInfoDto
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("slug")]
    public required string Slug { get; set; }
    [JsonPropertyName("logo")]
    public required ModAssetDto Logo { get; set; }
    [JsonPropertyName("summary")]
    public required string Summary { get; set; }
}

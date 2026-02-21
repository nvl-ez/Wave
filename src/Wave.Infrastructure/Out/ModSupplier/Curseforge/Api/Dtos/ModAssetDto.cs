using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public class ModAssetDto
{
    [JsonPropertyName("thumbnailUrl")]
    public required string ThumbnailUrl { get; set; }
}

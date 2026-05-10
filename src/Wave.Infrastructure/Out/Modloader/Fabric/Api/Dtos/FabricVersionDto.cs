using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Modloader.Fabric.Api.Dtos;

public record class FabricVersionJsonDto
{
    [JsonPropertyName("loader")]
    public required Loader Loader { get; set; }

}

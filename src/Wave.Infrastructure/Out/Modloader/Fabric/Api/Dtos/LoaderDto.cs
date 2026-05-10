using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Modloader.Fabric.Api.Dtos;

public class Loader
{
    [JsonPropertyName("version")]
    public required string Version { get; set; }
}

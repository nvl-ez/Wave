using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public record class ModDescriptionDto
{
    [JsonPropertyName("data")]
    public string Data { get; set; } = "";
}

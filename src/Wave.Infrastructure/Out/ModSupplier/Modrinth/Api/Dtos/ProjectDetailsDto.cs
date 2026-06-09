using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public class ProjectDetailsDto
{
    [JsonPropertyName("body")]
    public string Body { get; set; } = "";
}

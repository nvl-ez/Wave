using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

public record class FileDto
{
    [JsonPropertyName("url")]
    public required string DownloadUrl { get; set; }
    [JsonPropertyName("filename")]
    public required string Filename { get; set; }

}

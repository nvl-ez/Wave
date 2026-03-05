using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.Java.Adoptium.Dtos;

public record class BinaryDto
{
    [JsonPropertyName("os")]
    public required string Os { get; set; }
    [JsonPropertyName("architecture")]
    public required string Architecture { get; set; }
    [JsonPropertyName("image_type")]
    public required string ImageType { get; set; }
    [JsonPropertyName("c_lib")]
    public string? CLib { get; set; }
    [JsonPropertyName("jvm_impl")]
    public required string JvmImplementation { get; set; }
    [JsonPropertyName("package")]
    public PackageDto? Package { get; set; }
    [JsonPropertyName("installer")]
    public InstallerDto? Installer { get; set; }

}

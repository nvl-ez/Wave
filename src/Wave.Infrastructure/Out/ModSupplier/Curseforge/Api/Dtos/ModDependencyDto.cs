using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public class ModDependencyDto
{
    [JsonPropertyName("modId")]
    public required int ModId { get; set; }
    /*
    1 = EmbeddedLibrary
    2 = OptionalDependency
    3 = RequiredDependency
    4 = Tool
    5 = Incompatible
    6 = Include
    */
    [JsonPropertyName("relationType")]
    public required int FileRelationType { get; set; }
}

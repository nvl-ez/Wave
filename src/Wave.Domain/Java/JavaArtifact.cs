using System;
using Wave.Domain.System;

namespace Wave.Domain.Java;

public record class JavaArtifact
{
    public string Name => Type.ToString();
    public required JavaArtifactType Type { get; set; }
    public required string DownloadUrl { get; set; }
}

using System;
using Wave.Domain.Os;

namespace Wave.Domain.Java;

public record class JavaArtifact
{
    public required JavaArtifactType Type { get; set; }
    public required string DownloadUrl { get; set; }
}

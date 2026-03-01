using System;
using Wave.Domain.Os;

namespace Wave.Domain.Java;

public record class JavaArtifact
{
    public required FileType FileType { get; set; }
    public required string DownloadUrl { get; set; }
}

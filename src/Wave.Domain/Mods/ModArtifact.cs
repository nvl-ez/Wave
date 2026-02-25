using System;

namespace Wave.Domain.Mods;

public class ModArtifact
{
    public required string Filename { get; set; }
    public required string DownloadUrl { get; set; }
}

using System;

namespace Wave.Domain.Mods;

public class ModArtifact
{
    public required string FileName { get; set; }
    public required string DownloadUrl { get; set; }
}

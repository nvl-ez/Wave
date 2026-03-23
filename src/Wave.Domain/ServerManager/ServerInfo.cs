using System;

namespace Wave.Domain.ServerManager;

public sealed class ServerInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public string? ImageFilename { get; set; } = null;
    public string? ServerDirectory { get; set; } = null;
}

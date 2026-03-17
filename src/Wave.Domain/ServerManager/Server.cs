using System;

namespace Wave.Domain.ServerManager;

public record class Server
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Motd { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public Dictionary<string, string> Properties = new()
    {

    };
}

using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.Out.Minecraft;

public interface IServerPropertiesRepository
{
    public Task<IEnumerable<ServerPropertyDefinition>> GetAllServerPropertiesAsync(CancellationToken ct = default);
    public IEnumerable<ServerPropertyDefinition> GetAllServerProperties();

    public Task<IEnumerable<ServerPropertyDefinition>> GetServerPropertyAsync(string key, CancellationToken ct = default);
    public IEnumerable<ServerPropertyDefinition> GetServerProperty(string key);

}

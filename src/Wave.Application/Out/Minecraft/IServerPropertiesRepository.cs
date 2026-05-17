using System;
using Wave.Domain.ServerManager.Properties;

namespace Wave.Application.Out.Minecraft;

public interface IServerPropertyDefinitionRepository
{
    public Task<IEnumerable<PropertyDefinition>> GetAllServerPropertiesAsync(CancellationToken ct = default);
    public IEnumerable<PropertyDefinition> GetAllServerProperties();

    public Task<IEnumerable<PropertyDefinition>> GetServerPropertyAsync(string key, CancellationToken ct = default);
    public IEnumerable<PropertyDefinition> GetServerProperty(string key);

}

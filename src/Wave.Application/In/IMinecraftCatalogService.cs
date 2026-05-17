using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Properties;

namespace Wave.Application.In;

public interface IMinecraftCatalogService
{
    public Task<IEnumerable<MinecraftVersionInfo>> GetMinecraftVersionsAsync(MinecraftVersionQuery query, CancellationToken ct = default);
    public Task<IEnumerable<PropertyDefinition>> GetServerPropertyDefinitionsAsync(CancellationToken ct = default);
    public Task<PropertyDefinition> GetServerPropertyDefinitionAsync(string key, CancellationToken ct = default);

    public IEnumerable<MinecraftVersionInfo> GetMinecraftVersions(MinecraftVersionQuery query);
    public IEnumerable<PropertyDefinition> GetServerPropertyDefinitions();
    public PropertyDefinition GetServerPropertyDefinition(string key);
}

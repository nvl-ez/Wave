using System;
using Wave.Domain.Minecraft;

namespace Wave.Application.In;

public interface IMinecraftCatalogService
{
    public Task<IEnumerable<MinecraftVersion>> GetMinecraftVersionsAsync(MinecraftVersionQuery query, CancellationToken ct = default);
    public Task<IEnumerable<ServerPropertyDefinition>> GetServerPropertyDefinitionsAsync(CancellationToken ct = default);

    public IEnumerable<MinecraftVersion> GetMinecraftVersions(MinecraftVersionQuery query);
    public IEnumerable<ServerPropertyDefinition> GetServerPropertyDefinitions();
}

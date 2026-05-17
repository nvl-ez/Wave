using System;

using Wave.Application.In;
using Wave.Application.Out.Minecraft;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Properties;

namespace Wave.Infrastructure.In;

public class MinecraftCatalogService : IMinecraftCatalogService
{
    private readonly IMinecraftVersionRepository minecraftVersionRepository;
    private readonly IServerPropertyDefinitionRepository serverPropertiesRepository;

    public MinecraftCatalogService(IMinecraftVersionRepository minecraftVersionRepository, IServerPropertyDefinitionRepository serverPropertiesRepository)
    {
        this.minecraftVersionRepository = minecraftVersionRepository;
        this.serverPropertiesRepository = serverPropertiesRepository;
    }

    public async Task<IEnumerable<MinecraftVersionInfo>> GetMinecraftVersionsAsync(MinecraftVersionQuery query, CancellationToken ct = default)
    {
        return (await minecraftVersionRepository.GetAllVersionsAsync(ct))
            .Where(mv => query.IncludeSnapshots == true || mv.MinecraftVersionType == MinecraftVersionType.Release).ToList();
    }

    public IEnumerable<MinecraftVersionInfo> GetMinecraftVersions(MinecraftVersionQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PropertyDefinition>> GetServerPropertyDefinitionsAsync(CancellationToken ct = default)
    {
        return await serverPropertiesRepository.GetAllServerPropertiesAsync(ct);
    }

    public IEnumerable<PropertyDefinition> GetServerPropertyDefinitions()
    {
        return serverPropertiesRepository.GetAllServerProperties();
    }

    public async Task<PropertyDefinition> GetServerPropertyDefinitionAsync(string key, CancellationToken ct = default)
    {
        return (await serverPropertiesRepository.GetAllServerPropertiesAsync(ct)).First(sp => sp.Key == key);
    }

    public PropertyDefinition GetServerPropertyDefinition(string key)
    {
        return serverPropertiesRepository.GetAllServerProperties().First(sp => sp.Key == key);
    }
}

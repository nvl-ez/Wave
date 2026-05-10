using System;
using Wave.Application.In;
using Wave.Application.Out.Minecraft;
using Wave.Domain.Minecraft;

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

    public async Task<IEnumerable<MinecraftVersion>> GetMinecraftVersionsAsync(MinecraftVersionQuery query, CancellationToken ct = default)
    {
        return (await minecraftVersionRepository.GetAllVersionsAsync(ct))
            .Where(mv => query.IncludeSnapshots == true || mv.MinecraftVersionType == MinecraftVersionType.Release).ToList();
    }

    public IEnumerable<MinecraftVersion> GetMinecraftVersions(MinecraftVersionQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ServerPropertyDefinition>> GetServerPropertyDefinitionsAsync(CancellationToken ct = default)
    {
        return await serverPropertiesRepository.GetAllServerPropertiesAsync(ct);
    }

    public IEnumerable<ServerPropertyDefinition> GetServerPropertyDefinitions()
    {
        return serverPropertiesRepository.GetAllServerProperties();
    }

    public async Task<ServerPropertyDefinition> GetServerPropertyDefinitionAsync(string key, CancellationToken ct = default)
    {
        return (await serverPropertiesRepository.GetAllServerPropertiesAsync(ct)).First(sp => sp.Key == key);
    }

    public ServerPropertyDefinition GetServerPropertyDefinition(string key)
    {
        return serverPropertiesRepository.GetAllServerProperties().First(sp => sp.Key == key);
    }
}

using System;
using Wave.Application.Middle;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Middle;

public class PropertiesManagerService : IPropertiesManagerService
{
    private readonly IServerPathResolver serverPathResolver;
    private readonly IServerPropertiesRepository serverPropertiesRepository;

    public PropertiesManagerService(IServerPathResolver serverPathResolver, IServerPropertiesRepository serverPropertiesRepository)
    {
        this.serverPathResolver = serverPathResolver;
        this.serverPropertiesRepository = serverPropertiesRepository;
    }

    public async Task MergeSetPropertiesAsync(Server server, ServerQuery serverQuery, CancellationToken ct = default)
    {
        if (serverQuery.Properties is null) return;

        foreach (var kv in serverQuery.Properties)
        {
            server.Properties[kv.Key] = kv.Value;
        }

        await serverPropertiesRepository.SetAsync(serverPathResolver.GetServerPropertiesPath(server), server.Properties);
    }

    public async Task SetPropertiesAsync(Server server, CancellationToken ct = default)
    {
        await serverPropertiesRepository.SetAsync(serverPathResolver.GetServerPropertiesPath(server), server.Properties);
    }

    public async Task<Dictionary<string, string>> TryGetPropertiesAsync(Server server, CancellationToken ct = default)
    {
        string propertiesPath = serverPathResolver.GetServerPropertiesPath(server);
        Dictionary<string, string> properties;
        try
        {
            properties = await serverPropertiesRepository.GetAllAsync(propertiesPath);
        }
        catch (Exception ex)
        {
            if (ex is IOException || ex is InvalidDataException)
            {
                properties = server.Properties;
                await serverPropertiesRepository.SetAsync(propertiesPath, properties);
            }
            else
            {
                throw;
            }
        }
        return properties;
    }

    public async Task<string> TryGetPropertyAsync(Server server, string key, CancellationToken ct = default)
    {
        return (await TryGetPropertiesAsync(server))[key];
    }
}

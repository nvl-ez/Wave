using System;
using Wave.Application.Middle;
using Wave.Application.Out.Minecraft;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Middle;

public class PropertiesManagerService : IPropertiesManagerService
{
    private IServerPropertiesRepository serverPropertiesRepository;

    public PropertiesManagerService(IServerPropertiesRepository serverPropertiesRepository)
    {
        this.serverPropertiesRepository = serverPropertiesRepository;
    }

    public async Task MergeSetPropertiesAsync(Server server, CancellationToken ct = default)
    {
        Dictionary<string, string> current = server.Details.Properties;
        Dictionary<string, string> stored = await TryGetPropertiesAsync(server);

        foreach (var kv in current)
        {
            stored[kv.Key] = kv.Value;
        }

        await serverPropertiesRepository.SetAsync(server.PropertiesPath!, stored);
    }

    public async Task SetPropertiesAsync(Server server, CancellationToken ct = default)
    {
        await serverPropertiesRepository.SetAsync(server.PropertiesPath!, server.Details.Properties);
    }

    public async Task<Dictionary<string, string>> TryGetPropertiesAsync(Server server, CancellationToken ct = default)
    {
        Dictionary<string, string> properties;
        try
        {
            properties = await serverPropertiesRepository.GetAllAsync(server.PropertiesPath!);
        }
        catch (Exception ex)
        {
            if (ex is IOException || ex is InvalidDataException)
            {
                properties = server.Details.Properties;
                await serverPropertiesRepository.SetAsync(server.PropertiesPath!, properties);
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

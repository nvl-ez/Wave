using System;
using Wave.Application.Middle;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Middle;

public class EulaManagerService : IEulaManagerService
{
    private readonly IServerPathResolver serverPathResolver;
    private readonly IServerEulaRepository serverEulaRepository;

    public EulaManagerService(IServerPathResolver serverPathResolver, IServerEulaRepository serverEulaRepository)
    {
        this.serverPathResolver = serverPathResolver;
        this.serverEulaRepository = serverEulaRepository;
    }

    public async Task SetEulaAsync(Server server, ServerQuery serverQuery, CancellationToken ct = default)
    {
        await serverEulaRepository.SetAsync(serverPathResolver.GetEulaPath(server), serverQuery.Eula);
    }

    public async Task<bool> TryGetEulaAsync(Server server, CancellationToken ct = default)
    {
        bool eula = false;
        string eulaPath = serverPathResolver.GetEulaPath(server);

        try
        {
            eula = await serverEulaRepository.GetAsync(eulaPath);
        }
        catch (Exception ex)
        {
            if (ex is IOException || ex is InvalidDataException)
            {
                await serverEulaRepository.SetAsync(eulaPath, eula);
            }
        }
        return eula;
    }
}

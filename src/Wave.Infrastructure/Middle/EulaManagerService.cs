using System;
using Wave.Application.Middle;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Middle;

public class EulaManagerService : IEulaManagerService
{
    IServerEulaRepository serverEulaRepository;

    public EulaManagerService(IServerEulaRepository serverEulaRepository)
    {
        this.serverEulaRepository = serverEulaRepository;
    }

    public async Task SetEulaAsync(Server server, CancellationToken ct = default)
    {
        await serverEulaRepository.SetAsync(server.EulaPath!, server.Details.Eula);
    }

    public async Task<bool> TryGetEulaAsync(Server server, CancellationToken ct = default)
    {
        bool eula = false;

        try
        {
            eula = await serverEulaRepository.GetAsync(server.EulaPath!);
        }
        catch (Exception ex)
        {
            if (ex is IOException || ex is InvalidDataException)
            {
                await serverEulaRepository.SetAsync(server.EulaPath!, eula);
            }
        }
        return eula;
    }
}

using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Out.ServerManager;

public interface IServerRepository
{
    public Task<IEnumerable<Server>> GetAllAsync(CancellationToken ct = default);
    public Task SaveAsync(Server server, CancellationToken ct = default);
    public Task DeleteAsync(Guid id, CancellationToken ct = default);

    public IEnumerable<Server> GetAll();
    public void Save(Server server);
    public void Delete(Guid id);
}

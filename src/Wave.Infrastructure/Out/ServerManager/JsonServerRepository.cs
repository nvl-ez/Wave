using System;
using System.Text.Json;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Out.ServerManager;

public class JsonServerRepository : IServerRepository
{
    private readonly string repositoryPath;
    private const string fileName = "Servers.json";
    private readonly string filePath;
    public JsonServerRepository(string repositoryPath)
    {
        this.repositoryPath = repositoryPath;
        filePath = Path.Combine(this.repositoryPath, fileName);

        if (!File.Exists(filePath))
            File.Create(filePath).Dispose();
    }
    public async Task SaveAsync(Server server, CancellationToken ct = default)
    {
        //Remove if exists
        await DeleteAsync(server.Id, ct);

        //Load
        List<Server> servers = (List<Server>)await GetServersAsync(ct);

        //Add
        servers.Add(server);
        await WriteListToDiskAsync(servers, ct);

    }

    public async Task<IEnumerable<Server>> GetServersAsync(CancellationToken ct = default)
    {
        string json = await File.ReadAllTextAsync(filePath, ct);
        return json.Length != 0 ? JsonSerializer.Deserialize<List<Server>>(json)! : new List<Server>();
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        List<Server> servers = (List<Server>)await GetServersAsync(ct);
        if (servers.RemoveAll(s => s.Id == id) > 0)
        {
            await WriteListToDiskAsync(servers, ct);
        }
    }

    private async Task WriteListToDiskAsync(IEnumerable<Server> servers, CancellationToken ct = default)
    {
        string json = JsonSerializer.Serialize(servers, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, ct);
    }

    public IEnumerable<Server> GetServers()
    {
        string json = File.ReadAllText(filePath);
        return json.Length != 0 ? JsonSerializer.Deserialize<List<Server>>(json)! : new List<Server>();
    }

    public void Save(Server server)
    {
        //Remove if exists
        Delete(server.Id);

        //Load
        List<Server> servers = (List<Server>)GetServers();

        //Add
        servers.Add(server);
        WriteListToDisk(servers);
    }

    public void Delete(Guid id)
    {
        List<Server> servers = (List<Server>)GetServers();
        if (servers.RemoveAll(s => s.Id == id) > 0)
        {
            WriteListToDisk(servers);
        }
    }

    private void WriteListToDisk(IEnumerable<Server> servers)
    {
        string json = JsonSerializer.Serialize(servers, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }
}

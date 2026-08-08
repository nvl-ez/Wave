using System.Text.Json;
using Wave.Application.Out.Configuration;
using Wave.Domain.Configuration;

namespace Wave.Infrastructure.Out.Configuration;

public class JsonApplicationConfigurationRepository : IApplicationConfigurationRepository
{
    private const string FileName = "Configuration.json";
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly string filePath;

    public JsonApplicationConfigurationRepository(string repositoryPath)
    {
        filePath = Path.Combine(repositoryPath, FileName);
    }

    public async Task<ApplicationConfiguration> GetAsync(CancellationToken ct = default)
    {
        if (!File.Exists(filePath))
            return new ApplicationConfiguration();

        string json = await File.ReadAllTextAsync(filePath, ct);
        if (string.IsNullOrWhiteSpace(json))
            return new ApplicationConfiguration();

        return JsonSerializer.Deserialize<ApplicationConfiguration>(json) ?? new ApplicationConfiguration();
    }

    public async Task SaveAsync(ApplicationConfiguration configuration, CancellationToken ct = default)
    {
        string json = JsonSerializer.Serialize(configuration, SerializerOptions);
        await File.WriteAllTextAsync(filePath, json, ct);
    }
}

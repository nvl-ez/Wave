using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;

namespace Wave.Infrastructure.Out.Java.Repository;

public class JsonJavaRepository : IJavaInstallRepository
{
    private readonly string repositoryPath;
    private const string fileName = "IJavaInstallations.json";
    private readonly string filePath;
    public JsonJavaRepository(string repositoryPath)
    {
        this.repositoryPath = repositoryPath;
        filePath = Path.Combine(this.repositoryPath, fileName);

        if (!File.Exists(filePath))
            File.Create(filePath).Dispose();
    }

    public async Task AddAsync(IJavaInstallation javaInstallation, CancellationToken ct)
    {
        List<IJavaInstallation> javaInstallations = (List<IJavaInstallation>)await GetInstalledAsync(ct);
        if (!javaInstallations.Contains(javaInstallation))
        {
            javaInstallations.Add(javaInstallation);
            await WriteListToDisk(javaInstallations, ct);
        }
    }

    public async Task<IEnumerable<IJavaInstallation>> GetInstalledAsync(CancellationToken ct)
    {
        string json = await File.ReadAllTextAsync(filePath, ct);
        return json.Length != 0 ? JsonSerializer.Deserialize<List<IJavaInstallation>>(json)! : new List<IJavaInstallation>();
    }

    public async Task RemoveAsync(IJavaInstallation javaInstallation, CancellationToken ct)
    {
        List<IJavaInstallation> javaInstallations = (List<IJavaInstallation>)await GetInstalledAsync(ct);
        if (javaInstallations.Contains(javaInstallation))
        {
            javaInstallations.Remove(javaInstallation);
            await WriteListToDisk(javaInstallations, ct);
        }
    }

    private async Task WriteListToDisk(IEnumerable<IJavaInstallation> javaInstallations, CancellationToken ct)
    {
        string json = JsonSerializer.Serialize(javaInstallations, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, ct);
    }
}

using System;
using System.Text.Json;
using Wave.Application.Out.Java;
using Wave.Domain.Java;

namespace Wave.Infrastructure.Out.Java.Repository;

public class JavaJsonRepository : IJavaInstallRepository
{
    private readonly string repositoryPath;
    private const string fileName = "JavaInstallations.json";
    private readonly string filePath;
    public JavaJsonRepository(string repositoryPath)
    {
        this.repositoryPath = repositoryPath;
        filePath = Path.Combine(this.repositoryPath, fileName);

        if (!File.Exists(filePath))
            File.Create(filePath).Dispose();
    }

    public async Task AddAsync(JavaInstallation javaInstallation, CancellationToken ct)
    {
        List<JavaInstallation> javaInstallations = (List<JavaInstallation>)await GetInstalledAsync(ct);
        if (!javaInstallations.Contains(javaInstallation))
        {
            javaInstallations.Add(javaInstallation);
            await WriteListToDisk(javaInstallations, ct);
        }
    }

    public async Task<IEnumerable<JavaInstallation>> GetInstalledAsync(CancellationToken ct)
    {
        string json = await File.ReadAllTextAsync(filePath, ct);
        return json.Length != 0 ? JsonSerializer.Deserialize<List<JavaInstallation>>(json)! : new List<JavaInstallation>();
    }

    public async Task RemoveAsync(JavaInstallation javaInstallation, CancellationToken ct)
    {
        List<JavaInstallation> javaInstallations = (List<JavaInstallation>)await GetInstalledAsync(ct);
        if (javaInstallations.Contains(javaInstallation))
        {
            javaInstallations.Remove(javaInstallation);
            await WriteListToDisk(javaInstallations, ct);
        }
    }

    private async Task WriteListToDisk(IEnumerable<JavaInstallation> javaInstallations, CancellationToken ct)
    {
        string json = JsonSerializer.Serialize(javaInstallations, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, ct);
    }
}

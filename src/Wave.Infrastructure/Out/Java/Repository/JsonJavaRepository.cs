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

    public async Task AddAsync(Domain.Java.JavaInstallation javaInstallation, CancellationToken ct)
    {
        List<Domain.Java.JavaInstallation> javaInstallations = (List<Domain.Java.JavaInstallation>)await GetAllAsync(ct);
        if (!javaInstallations.Contains(javaInstallation))
        {
            javaInstallations.Add(javaInstallation);
            await WriteListToDisk(javaInstallations, ct);
        }
    }

    public async Task<IEnumerable<Domain.Java.JavaInstallation>> GetAllAsync(CancellationToken ct)
    {
        string json = await File.ReadAllTextAsync(filePath, ct);
        return json.Length != 0 ? JsonSerializer.Deserialize<List<Domain.Java.JavaInstallation>>(json)! : new List<Domain.Java.JavaInstallation>();
    }

    public async Task RemoveAsync(Domain.Java.JavaInstallation javaInstallation, CancellationToken ct)
    {
        List<Domain.Java.JavaInstallation> javaInstallations = (List<Domain.Java.JavaInstallation>)await GetAllAsync(ct);
        if (javaInstallations.Contains(javaInstallation))
        {
            javaInstallations.Remove(javaInstallation);
            await WriteListToDisk(javaInstallations, ct);
        }
    }

    private async Task WriteListToDisk(IEnumerable<Domain.Java.JavaInstallation> javaInstallations, CancellationToken ct)
    {
        string json = JsonSerializer.Serialize(javaInstallations, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, json, ct);
    }
}

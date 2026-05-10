using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Out.ServerManager;

public class ServerEulaRepository : IServerEulaRepository
{
    private const string eulaPattern = @"(?m)^\s*eula\s*=\s*(true|false)\s*$";
    public async Task<bool> GetAsync(string eulaPath, CancellationToken ct = default)
    {
        if (eulaPath is null)
            throw new NullReferenceException("Eula file path is null.");
        if (!File.Exists(eulaPath))
            throw new IOException($"File '{eulaPath}' does not exist");

        string eulaText = await File.ReadAllTextAsync(eulaPath);
        if (string.IsNullOrEmpty(eulaText))
            throw new InvalidDataException("Eula file is empty.");

        var match = Regex.Match(eulaText, eulaPattern);

        bool eula = false;

        if (match.Success && bool.TryParse(match.Groups[1].Value, out var parsed))
        {
            eula = parsed;
        }
        return eula;
    }

    public async Task SetAsync(string eulaPath, bool value, CancellationToken ct = default)
    {
        if (eulaPath is null)
            throw new NullReferenceException("Eula file path is null.");

        await File.WriteAllTextAsync(eulaPath, BuildFileContent(value).ToString());
    }

    private StringBuilder BuildFileContent(bool value)
    {
        StringBuilder content = new();
        string textValue = value ? "true" : "false";

        string stringDate = DateTime.Now.ToString("ddd MMM dd HH:mm:ss 'CET' yyyy", CultureInfo.InvariantCulture);

        content.Append($"#By changing the setting below to TRUE you are indicating your agreement to our EULA (https://aka.ms/MinecraftEULA).\n#{stringDate}\n");
        content.Append($"eula={textValue}\n");

        return content;
    }
}

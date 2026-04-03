using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Wave.Application.Out.ServerManager;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Out.ServerManager;

public class ServerPropertiesRepository : IServerPropertiesRepository
{
    public async Task<Dictionary<string, string>> GetAllAsync(Server server, CancellationToken ct = default)
    {
        var propertiesFile = await ValidateServerProperties(server);

        string propertiesPattern = @"^(?<key>[^=\r\n]+)=(?<value>[^\r\n]*)$";

        string propertiesText = await File.ReadAllTextAsync(propertiesFile);

        var matches = Regex.Matches(propertiesText, propertiesPattern, RegexOptions.Multiline);

        var properties = new Dictionary<string, string>();

        foreach (Match match in matches)
        {
            string key = match.Groups["key"].Value;
            string value = match.Groups["value"].Value;
            properties[key] = value;
        }

        return properties;
    }

    public async Task<string> GetAsync(Server server, string key, CancellationToken ct = default)
    {
        return (await GetAllAsync(server))[key];
    }

    public async Task SetAsync(Server server, CancellationToken ct = default)
    {
        Dictionary<string, string> properties = server.Details.Properties;

        var propertiesFile = await ValidateServerProperties(server);

        Dictionary<string, string> existingProperties = await GetAllAsync(server);

        foreach (var property in properties)
        {
            if (existingProperties.ContainsKey(property.Key))
            {
                existingProperties[property.Key] = property.Value;
            }
            else
            {
                existingProperties.Add(property.Key, property.Value);
            }
        }

        await File.WriteAllTextAsync(propertiesFile, BuildFileContent(existingProperties).ToString());
    }

    public async Task SetAsync(Server server, string key, string value, CancellationToken ct = default)
    {
        var propertiesFile = await ValidateServerProperties(server);

        Dictionary<string, string> existingProperties = await GetAllAsync(server);

        if (existingProperties.ContainsKey(key))
        {
            existingProperties[key] = value;
        }
        else
        {
            existingProperties.Add(key, value);
        }

        await File.WriteAllTextAsync(propertiesFile, BuildFileContent(existingProperties).ToString());
    }

    private StringBuilder BuildFileContent(Dictionary<string, string> properties)
    {
        StringBuilder content = new();

        string stringDate = DateTime.Now.ToString("ddd MMM dd HH:mm:ss 'CET' yyyy", CultureInfo.InvariantCulture);

        content.Append($"#Minecraft server properties\n#{stringDate}\n");

        foreach (var property in properties)
        {
            content.Append($"{property.Key}={property.Value}\n");
        }

        return content;
    }

    private async Task<string> ValidateServerProperties(Server server)
    {
        ServerInfo info = server.Info;
        ServerDetails details = server.Details;

        if (info.ServerDirectory is null) throw new NullReferenceException("Server Directory cannot be null.");
        if (details.PropertiesFilename is null) throw new NullReferenceException("Server Properties Filename cannot be null.");

        string propertiesFile = Path.Combine(info.ServerDirectory, details.PropertiesFilename);

        if (!File.Exists(propertiesFile))
        {
            await File.WriteAllTextAsync(propertiesFile, BuildFileContent(server.Details.Properties).ToString());
        }

        return propertiesFile;
    }
}

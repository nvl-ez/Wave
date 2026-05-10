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
    private const string propertiesPattern = @"^(?<key>[^=\r\n]+)=(?<value>[^\r\n]*)\r?$";

    //Prevalece lo que hay en los archivos del server
    public async Task<Dictionary<string, string>> GetAllAsync(string propertiesPath, CancellationToken ct = default)
    {
        if (propertiesPath is null) throw new NullReferenceException("server.properties path cannot be null");
        if (!File.Exists(propertiesPath)) throw new IOException($"The file {propertiesPath} does not exist");

        string propertiesText = await File.ReadAllTextAsync(propertiesPath);

        if (string.IsNullOrEmpty(propertiesText)) throw new InvalidDataException("server.properties file is empty");

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

    public async Task<string> GetAsync(string propertiesPath, string key, CancellationToken ct = default)
    {
        return (await GetAllAsync(propertiesPath))[key];
    }


    public async Task SetAsync(string propertiesPath, Dictionary<string, string> properties, CancellationToken ct = default)
    {
        if (propertiesPath is null) throw new NullReferenceException("server.properties path cannot be null");

        await File.WriteAllTextAsync(propertiesPath, BuildFileContent(properties).ToString());
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
}

using System;
using Wave.Application.Middle;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Middle;

public class ServerPathResolver : IServerPathResolver
{
    private readonly string serversDirectory;

    public ServerPathResolver(string serversDirectory)
    {
        this.serversDirectory = serversDirectory;
    }

    public string GetServerRootDirectory(Server server)
    {
        return Path.Combine(serversDirectory, CleanseFileName(server.Id.ToString()));
    }

    public string CreateServerRootDirectory(Server server)
    {
        string directory = GetServerRootDirectory(server);
        Directory.CreateDirectory(directory);
        return directory;
    }

    public string GetServerJarPath(Server server)
    {
        return Path.Combine(GetServerRootDirectory(server), server.ServerPaths.ServerJarFilename);
    }

    public string GetServerPropertiesPath(Server server)
    {
        string serverDirectory = GetServerRootDirectory(server);
        return Path.Combine(serverDirectory, server.ServerPaths.PropertiesFileName);
    }

    public string CreateServerPropertiesFile(Server server)
    {
        string file = GetServerPropertiesPath(server);
        File.Create(file).Close();
        return file;
    }

    public string GetEulaPath(Server server)
    {
        string serverDirectory = GetServerRootDirectory(server);
        return Path.Combine(serverDirectory, server.ServerPaths.EulaFileName);
    }

    public string CreateEulaFile(Server server)
    {
        string file = GetEulaPath(server);
        File.Create(file).Close();
        return file;
    }

    public string GetModsDirectory(Server server)
    {
        throw new NotImplementedException();
    }

    public string CreateModsDirectory(Server server)
    {
        throw new NotImplementedException();
    }

    private static string CleanseFileName(string FileName)
    {
        var invalids = System.IO.Path.GetInvalidFileNameChars();
        return String.Join("_", FileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
    }
}

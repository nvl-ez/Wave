using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.Middle;

public interface IServerPathResolver
{
    public string GetServerRootDirectory(Server server);
    public string CreateServerRootDirectory(Server server);
    public string GetServerPropertiesPath(Server server);
    public string CreateServerPropertiesFile(Server server);
    public string GetServerJarPath(Server server);
    public string GetEulaPath(Server server);
    public string CreateEulaFile(Server server);
    public string GetModsDirectory(Server server);
    public string CreateModsDirectory(Server server);
}

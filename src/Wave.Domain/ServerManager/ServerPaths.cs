using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wave.Domain.ServerManager
{
    public class ServerPaths
    {
        public string PropertiesFileName { get; set; } = "server.properties";
        public string EulaFileName { get; set; } = "eula.txt";
        public string ServerJarFilename { get; set; } = "server.jar";
        public string ModloaderJarFileName { get; set; } = "modloader.jar";
        public string ImageFilename { get; set; } = "";
        public string ModDirectoryName { get; set; } = "mods";
    }
}
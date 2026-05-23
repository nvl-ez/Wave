using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Infrastructure.Out.Modloader.Forge.Api;

public static class Mapper
{
    public static ModloaderInfo ToDomain(string dto, MinecraftVersionInfo minecraftVersionInfo) //TODO: SACAR MINECRAFT VERSION DE AQUI: es incorrecto porque no todas las versiones de forge son de la version actual.
    {
        int charLocation = dto.IndexOf('-');
        string mcVersion = dto.Substring(0, charLocation);
        string version = dto.Substring(charLocation + 1, (dto.Length - charLocation) - 1);

        return new ModloaderInfo()
        {
            Version = version,
            MinecraftVersion = minecraftVersionInfo.MinecraftVersion,
            DowloadUrl = $"https://maven.minecraftforge.net/net/minecraftforge/forge/{dto}/forge-{dto}-installer.jar",
            ModloaderType = ModloaderType.Forge
        };
    }
}

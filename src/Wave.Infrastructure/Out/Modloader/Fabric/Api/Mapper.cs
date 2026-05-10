using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Infrastructure.Out.Modloader.Fabric.Api.Dtos;

namespace Wave.Infrastructure.Out.Modloader.Fabric.Api;

public static class Mapper
{
    public static ModloaderInfo ToDomain(FabricVersionJsonDto dto, MinecraftVersion minecraftVersion)
    {
        return new ModloaderInfo()
        {
            Version = dto.Loader.Version,
            MinecraftVersion = minecraftVersion,
            DowloadUrl = $"https://maven.fabricmc.net/net/fabricmc/fabric-installer/1.1.1/fabric-installer-1.1.1.jar",
            ModloaderType = ModloaderType.Fabric
        };
    }
}

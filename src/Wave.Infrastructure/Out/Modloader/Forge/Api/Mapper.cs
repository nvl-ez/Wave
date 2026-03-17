using System;
using Wave.Domain.Modloaders;

namespace Wave.Infrastructure.Out.Modloader.Forge.Api;

public static class Mapper
{
    public static ForgeVersion ToDomain(string dto)
    {
        int charLocation = dto.IndexOf('-');
        string minecraftVersion = dto.Substring(0, charLocation);
        string version = dto.Substring(charLocation + 1, (dto.Length - charLocation) - 1);

        return new ForgeVersion()
        {
            Version = version,
            MinecraftVersion = minecraftVersion,
            DowloadUrl = $"https://maven.minecraftforge.net/net/minecraftforge/forge/{dto}/forge-{dto}-installer.jar"
        };
    }
}

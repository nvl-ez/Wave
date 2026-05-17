using System;
using Wave.Domain.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Versions.MinecraftVersionDtos;

namespace Wave.Infrastructure.Out.Minecraft.Api;

public static class Mapper
{
    public static MinecraftVersionInfo ToDomain(MinecraftVersionJson dto)
    {
        return new MinecraftVersionInfo()
        {
            MinecraftVersion = dto.Id,
            MinecraftVersionType = dto.Type == "release" ? MinecraftVersionType.Release :
                (dto.Type == "snapshot" ? MinecraftVersionType.Snapshot : MinecraftVersionType.Other),
            DetailsUrl = dto.Url,
            ReleaseDate = dto.ReleaseTime
        };
    }
}

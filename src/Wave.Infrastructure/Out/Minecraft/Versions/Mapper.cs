using System;
using Wave.Domain.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Versions.Dtos;

namespace Wave.Infrastructure.Out.Minecraft.Api;

public static class Mapper
{
    public static MinecraftVersion ToDomain(MinecraftVersionJson dto)
    {
        return new MinecraftVersion()
        {
            Version = dto.Id,
            MinecraftVersionType = dto.Type == "release" ? MinecraftVersionType.Release :
                (dto.Type == "snapshot" ? MinecraftVersionType.Snapshot : MinecraftVersionType.Other),
            DetailsUrl = dto.Url,
            ReleaseDate = dto.ReleaseTime
        };
    }
}

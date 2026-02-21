using System;
using Wave.Domain.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Api.Dtos;

namespace Wave.Infrastructure.Out.Minecraft.Api.Mappers;

public static class MinecraftVersionJsonMapper
{
    public static MinecraftVersion ToDomain(MinecraftVersionJson dto)
    {
        return new MinecraftVersion()
        {
            Version = dto.Id,
            VersionType = dto.Type == "release" ? MinecraftVersion.VersionTypeEnum.Release :
                (dto.Type == "snapshot" ? MinecraftVersion.VersionTypeEnum.Snapshot : MinecraftVersion.VersionTypeEnum.Other),
            DetailsUrl = new Uri(dto.Url),
            ReleaseDate = dto.ReleaseTime
        };
    }
}

using System;
using Wave.Domain.Mods;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Mappers;

public static class Mapper
{
    public static ModInfo ToDomain(ProjectDto dto, ModSupplierQuery query)
    {
        return new ModInfo()
        {
            Name = dto.Title,
            ExternalId = dto.ProjectId,
            MinecraftVersion = query.MinecraftVersion,
            ModSupplierType = ModSupplierType.Modrinth,
            ModloaderType = query.ModloaderType,
            IconUrl = dto.IconUrl == null ? null : new Uri(dto.IconUrl),
            Slug = dto.Slug
        };
    }

    public static ModVersion ToDomain(ProjectVersionDto versionDto, FileDto fileDto, ModInfo modInfoResult)
    {
        ModVersion mod = new ModVersion()
        {
            Name = versionDto.Name,
            ExternalId = dto.ProjectId,
            MinecraftVersion = query.MinecraftVersion,
            ModSupplierType = ModSupplierType.Modrinth,
            ModloaderType = query.ModloaderType,
            IconUrl = dto.IconUrl == null ? null : new Uri(dto.IconUrl),
            Slug = dto.Slug
        };
    }
}

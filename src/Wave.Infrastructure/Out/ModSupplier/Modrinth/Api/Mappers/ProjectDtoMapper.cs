using System;
using Wave.Domain.Mods;
using Wave.Domain.ModSupplier;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Mappers;

public static class ProjectDtoMapper
{
    public static ModInfoResult ToDomain(ProjectDto dto, ModSupplierQuery query)
    {
        return new ModInfoResult()
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

    public static Mod ToDomain(ProjectVersionDto versionDto, FileDto fileDto, ModInfoResult modInfoResult)
    {
        Mod mod = new Mod()
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

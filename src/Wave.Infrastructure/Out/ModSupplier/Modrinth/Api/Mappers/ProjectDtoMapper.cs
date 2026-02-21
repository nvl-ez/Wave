using System;
using Wave.Domain.Mods;
using Wave.Domain.ModSupplier;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Mappers;

public static class ProjectDtoMapper
{
    public static Mod ToDomain(ProjectDto dto, ModSupplierQuery query)
    {
        return new Mod()
        {
            Name = dto.Title,
            ExternalId = dto.ProjectId,
            MinecraftVersion = query.MinecraftVersion,
            ModSupplierType = ModSupplierType.Modrinth,
            ModloaderType = query.ModloaderType,
            IconUrl = dto.IconUrl == null ? null : new Uri(dto.IconUrl)
        };
    }
}

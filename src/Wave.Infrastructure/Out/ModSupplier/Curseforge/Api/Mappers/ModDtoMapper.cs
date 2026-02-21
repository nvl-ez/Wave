using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Mods;
using Wave.Domain.ModSupplier;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Mappers;

public static class ModDtoMapper
{
    public static Mod ToDomain(ModDto dto, ModSupplierQuery modSupplierQuery)
    {
        return new Mod()
        {
            ExternalId = dto.Id.ToString(),
            MinecraftVersion = modSupplierQuery.MinecraftVersion,
            ModloaderType = modSupplierQuery.ModloaderType,
            ModSupplierType = ModSupplierType.Curseforge,
            Name = dto.Name,
            IconUrl = new Uri(dto.Logo.ThumbnailUrl)
        };
    }
}

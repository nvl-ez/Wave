using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Mods;
using Wave.Domain.ModSupplier;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Mappers;

public static class ModDtoMapper
{
    public static ModInfoResult ToDomain(ModInfoDto dto, ModSupplierQuery modSupplierQuery)
    {
        return new ModInfoResult()
        {
            ExternalId = dto.Id.ToString(),
            MinecraftVersion = modSupplierQuery.MinecraftVersion,
            ModloaderType = modSupplierQuery.ModloaderType,
            ModSupplierType = ModSupplierType.Curseforge,
            Name = dto.Name,
            IconUrl = new Uri(dto.Logo.ThumbnailUrl),
            Slug = dto.Slug
        };
    }

    public static Mod ToDomain(ModFileDto dto, ModInfoResult modInfoResult)
    {
        return new Mod()
        {
            ExternalId = dto.ModId.ToString(),
            MinecraftVersion = modInfoResult.MinecraftVersion,
            ModloaderType = modInfoResult.ModloaderType,
            ModSupplierType = ModSupplierType.Curseforge,
            Name = dto.DisplayName,
            IconUrl = modInfoResult.IconUrl,
            DownloadUrl = new Uri(dto.DownloadUrl),
            Version = ""
        };
    }
}

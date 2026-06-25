using System;
using Wave.Domain.Minecraft;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager.Modloader;
using Wave.Domain.Utils;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api;

public static class Mapper
{
    public static ModInfo ToDomain(ModInfoDto dto)
    {
        return new(dto.Id.ToString(), dto.Name, dto.Slug, ModSupplierType.Curseforge, dto.Summary, dto.Logo.ThumbnailUrl);
    }
    public static PaginationState ToDomain(PaginationDto paginationDto)
    {
        return new PaginationState()
        {
            Index = paginationDto.Index,
            ResultCount = paginationDto.ResultCount,
            TotalCount = paginationDto.TotalCount
        };
    }

    public static ModVersion ToDomain(ModFileDto dto, ModVersionSupplierQuery modVersionSupplierQuery)
    {
        List<ModArtifact> artifacts = new List<ModArtifact>();
        ModArtifact artifact = new()
        {
            FileName = dto.FileName,
            DownloadUrl = dto.DownloadUrl
        };
        artifacts.Add(artifact);

        List<ModDependency> dependencies = new List<ModDependency>();
        if (dto.Dependencies is not null && dto.Dependencies.Count > 0)
        {
            foreach (ModDependencyDto dependencyDto in dto.Dependencies)
            {
                dependencies.Add(new ModDependency()
                {
                    DependencyType = ToDomainModDependencyType(dependencyDto.FileRelationType),
                    ModId = dependencyDto.ModId.ToString()
                });
            }
        }

        ModVersion mod = new ModVersion()
        {
            VersionId = dto.FileId.ToString(),
            VersionName = dto.DisplayName,
            Dependencies = dependencies,
            Artifacts = artifacts,
            MinecraftVersion = modVersionSupplierQuery.MinecraftVersion,
            ModloaderType = modVersionSupplierQuery.ModloaderType,
            ModSupplierType = ModSupplierType.Curseforge,
            ModVersionType = ToDomainModVersionType(dto.ReleaseType),
            Version = dto.DisplayName
        };
        return mod;
    }

    public static ModVersionType ToDomainModVersionType(int versionType)
    {
        switch (versionType)
        {
            case 1:
                return ModVersionType.Release;
            case 2:
                return ModVersionType.Beta;
            case 3:
                return ModVersionType.Alpha;
            default:
                throw new NotImplementedException("Missing implementation for mod version type.");
        }

    }

    public static int ToDtoModloaderType(ModloaderType modloaderType)
    {
        switch (modloaderType)
        {
            case ModloaderType.Forge:
                return 1;
            case ModloaderType.Fabric:
                return 4;
            default:
                throw new NotImplementedException("Missing implementation for modloader.");
        }

    }

    public static ModDependencyType ToDomainModDependencyType(int dependencyTpe)
    {
        switch (dependencyTpe)
        {
            case 1:
                throw new NotImplementedException("Missing implementation for 'EmbeddedLibrary' dependency type.");
            case 2:
                return ModDependencyType.Optional;
            case 3:
                return ModDependencyType.Required;
            case 4:
                throw new NotImplementedException("Missing implementation for 'Tool' dependency type.");
            case 5:
                return ModDependencyType.Incompatible;
            case 6:
                throw new NotImplementedException("Missing implementation for 'Include' dependency type.");
            default:
                throw new NotImplementedException("Missing implementation for dependency type.");
        }
    }
}

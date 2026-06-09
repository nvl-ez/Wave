using System;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager.Modloader;
using Wave.Domain.Utils;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api;

public static class Mapper
{
    public static ModInfo ToDomain(ProjectDto dto, ModInfoSupplierQuery query)
    {
        return new()
        {
            Name = dto.Title,
            ModId = dto.ProjectId,
            ModSupplierType = ModSupplierType.Modrinth,
            IconUrl = dto.IconUrl,
            Slug = dto.Slug,
            Summary = dto.Description
        };
    }

    public static PaginationState ToDomain(SearchModsResponseDto searchModsResponseDto)
    {
        return new()
        {
            Index = searchModsResponseDto.Offset,
            ResultCount = searchModsResponseDto.Limit,
            TotalCount = searchModsResponseDto.TotalHits
        };
    }

    public static ModVersion ToDomain(ProjectVersionDto dto, ModVersionSupplierQuery modInfoResult)
    {
        List<ModArtifact> artifacts = new List<ModArtifact>();
        if (dto.Files is not null && dto.Files.Count > 0)
        {
            foreach (FileDto fileDto in dto.Files)
            {
                artifacts.Add(new ModArtifact()
                {
                    Filename = fileDto.Filename,
                    DownloadUrl = fileDto.DownloadUrl
                });
            }
        }

        List<ModDependency> dependencies = new List<ModDependency>();
        if (dto.Dependencies is not null && dto.Dependencies.Count > 0)
        {
            foreach (ModDependencyDto dependencyDto in dto.Dependencies)
            {
                dependencies.Add(new ModDependency()
                {
                    DependencyType = ToDomainModDependencyType(dependencyDto.DependencyType),
                    ModId = dependencyDto.ProjectId.ToString()
                });
            }
        }

        ModVersion mod = new ModVersion()
        {
            VersionName = dto.Name,
            MinecraftVersion = modInfoResult.MinecraftVersion,
            ModSupplierType = ModSupplierType.Modrinth,
            ModloaderType = modInfoResult.ModloaderType,
            Version = dto.Version,
            VersionId = dto.VersionId,
            Artifacts = artifacts,
            Dependencies = dependencies,
            Changelog = dto.Changelog,
            ModVersionType = ToDomainModVersionType(dto.VersionType),
            Featured = dto.Featured

        };
        return mod;
    }

    public static ModVersionType ToDomainModVersionType(string versionType)
    {
        switch (versionType)
        {
            case "release":
                return ModVersionType.Release;
            case "beta":
                return ModVersionType.Beta;
            case "alpha":
                return ModVersionType.Alpha;
            default:
                throw new NotImplementedException("Missing implementation for mod version type.");
        }

    }
    public static ModDependencyType ToDomainModDependencyType(string dependencyType)
    {
        switch (dependencyType)
        {
            case "required":
                return ModDependencyType.Required;
            case "optional":
                return ModDependencyType.Optional;
            case "incompatible":
                return ModDependencyType.Incompatible;
            case "embedded":
                throw new NotImplementedException("Missing implementation for 'embedded' dependency type.");
            default:
                throw new NotImplementedException("Missing implementation for dependency type.");
        }
    }
    public static string ToDtoModloaderType(ModloaderType modloaderType)
    {
        switch (modloaderType)
        {
            case ModloaderType.Forge:
                return "forge";
            case ModloaderType.Fabric:
                return "fabric";
            default:
                throw new NotImplementedException("Missing implementation for modloader.");
        }
    }
}

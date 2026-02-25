using System;
using Wave.Domain.Modloaders;
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
            ModId = dto.ProjectId,
            MinecraftVersion = query.MinecraftVersion,
            ModSupplierType = ModSupplierType.Modrinth,
            ModloaderType = query.ModloaderType,
            IconUrl = dto.IconUrl,
            Slug = dto.Slug,
            Description = dto.Description
        };
    }

    public static ModVersion ToDomain(ProjectVersionDto dto, ModInfo modInfoResult)
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
            Name = dto.Name,
            ModId = dto.ProjectId,
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
            case ModloaderType.Vanilla:
                throw new NotSupportedException("Mods cannot be searched for Vanilla Minecraft.");
            default:
                throw new NotImplementedException("Missing implementation for modloader.");
        }
    }
}

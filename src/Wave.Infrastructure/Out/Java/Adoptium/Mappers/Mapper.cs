using System;
using Wave.Domain.Java;
using Wave.Domain.Os;
using Wave.Infrastructure.Out.Java.Adoptium.Dtos;

namespace Wave.Infrastructure.Out.Java.Adoptium.Mappers;

public static class Mapper
{
    public static JavaVersion ToDomain(LatestAssetDto dto)
    {
        if (dto.Binary.Installer is null && dto.Binary.Package is null)
            throw new NullReferenceException("Installer and Package cannot be null at the same time.");

        List<JavaArtifact> artifacts = new List<JavaArtifact>();

        if (dto.Binary.Installer is not null)
        {
            artifacts.Add(new()
            {
                FileType = FileType.Installer,
                DownloadUrl = dto.Binary.Installer.DownloadUrl
            });
        }

        if (dto.Binary.Package is not null)
        {
            artifacts.Add(new()
            {
                FileType = FileType.Compressed,
                DownloadUrl = dto.Binary.Package.DownloadUrl
            });
        }


        return new JavaVersion()
        {
            Version = dto.Version.Major,
            ArchitectureBitType = ToDomainArchitectureBitType(dto.Binary.Architecture),
            ArchitectureType = ToDomainArchitectureType(dto.Binary.Architecture),
            OsType = ToDomainOsType(dto.Binary.Os),
            JavaArtifacts = artifacts
        };
    }

    public static int ToDomainArchitectureBitType(string architecture)
    {
        switch (architecture)
        {
            case "x64":
                return 64;
            case "x32":
                return 32;
            case "x86":
                return 32;
            case "aarch64":
                return 64;
            case "arm":
                return 32;
            default:
                throw new NotImplementedException("Missing implementation for Architecture.");
        }
    }

    public static ArchitectureType ToDomainArchitectureType(string architecture)
    {
        switch (architecture)
        {
            case "x64":
                return ArchitectureType.X86;
            case "x32":
                return ArchitectureType.X86;
            case "x86":
                return ArchitectureType.X86;
            case "aarch64":
                return ArchitectureType.Arm;
            case "arm":
                return ArchitectureType.Arm;
            default:
                throw new NotImplementedException("Missing implementation for ArchitectureType.");
        }
    }

    public static string ToDtoArchitectureType(ArchitectureType architectureType)
    {
        switch (architectureType)
        {
            case ArchitectureType.X86:
                return "x64";
            case ArchitectureType.Arm:
                return "aarch64";
            default:
                throw new NotImplementedException("Missing implementation for ArchitectureType.");
        }
    }

    public static string ToDtoOsType(OsType osType)
    {
        switch (osType)
        {
            case OsType.Windows:
                return "windows";
            case OsType.Linux:
                return "linux";
            case OsType.MacOs:
                return "mac";
            default:
                throw new NotImplementedException("Missing implementation for OsType.");
        }
    }

    public static OsType ToDomainOsType(string os)
    {
        switch (os)
        {
            case "windows":
                return OsType.Windows;
            case "linux":
                return OsType.Linux;
            case "mac":
                return OsType.MacOs;
            default:
                throw new NotImplementedException("Missing implementation for OsType.");
        }
    }
}

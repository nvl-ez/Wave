using System;
using System.Numerics;
using System.Text.RegularExpressions;
using Wave.Domain.Java;
using Wave.Domain.Os;
using Wave.Infrastructure.Out.Java.Mojang.Dtos;

namespace Wave.Infrastructure.Out.Java.Mojang.Mappers;

public static class Mapper
{
    private const string versionPattern = @"^(?<major>\d+)(?:\.(?<minor>\d+))?(?:\.(?<security>\d+))?(?:\.(?<build>\d+))?";
    private const string legacyVersionPattern = @"^(?<major>\d+)u(?<security>\d+)(?:b(?<build>\d+))?";
    public static JavaVersion ToDomain(string platform, string name, ReleaseDto dto)
    {
        if (dto is null) throw new NullReferenceException("DTO cannot be null");

        Match regex = Regex.Match(dto.Version.Name, versionPattern);
        if (!regex.Success)
        {
            regex = Regex.Match(dto.Version.Name, legacyVersionPattern);
        }

        if (!regex.Success) throw new NotImplementedException("Version string does not match any pattern.");

        return new JavaVersion()
        {
            Version = int.Parse(regex.Groups["major"].Value),
            ArchitectureType = ToDomainArchitectureType(platform),
            ArchitectureBitType = ToDomainArchitectureBitType(platform),
            OsType = ToDomainOsType(platform),
            Name = name,
            JavaArtifacts = [
                new JavaArtifact(){
                    Type = JavaArtifactType.Manifest,
                    DownloadUrl = dto.Manifest.Url
                }
            ]
        };
    }

    public static ArchitectureType ToDomainArchitectureType(string platform)
    {
        switch (platform)
        {
            case "gamecore":
                throw new NotImplementedException("gamecore ArchitectureType not supported.");
            case "linux":
                return ArchitectureType.X86;
            case "linux-i386":
                return ArchitectureType.X86;
            case "mac-os":
                return ArchitectureType.X86;
            case "mac-os-arm64":
                return ArchitectureType.Arm;
            case "windows-arm64":
                return ArchitectureType.Arm;
            case "windows-x64":
                return ArchitectureType.X86;
            case "windows-x86":
                return ArchitectureType.X86;
            default:
                throw new NotImplementedException("ArchitectureType not supported.");
        }
    }

    public static int ToDomainArchitectureBitType(string platform)
    {
        switch (platform)
        {
            case "gamecore":
                throw new NotImplementedException("gamecore ArchitectureType not supported.");
            case "linux":
                return 64;
            case "linux-i386":
                return 32;
            case "mac-os":
                return 64;
            case "mac-os-arm64":
                return 64;
            case "windows-arm64":
                return 64;
            case "windows-x64":
                return 64;
            case "windows-x86":
                return 32;
            default:
                throw new NotImplementedException("ArchitectureType not supported.");
        }
    }

    public static OsType ToDomainOsType(string platform)
    {
        switch (platform)
        {
            case "gamecore":
                throw new NotImplementedException("gamecore ArchitectureType not supported.");
            case "linux":
                return OsType.Linux;
            case "linux-i386":
                return OsType.Linux;
            case "mac-os":
                return OsType.MacOs;
            case "mac-os-arm64":
                return OsType.MacOs;
            case "windows-arm64":
                return OsType.Windows;
            case "windows-x64":
                return OsType.Windows;
            case "windows-x86":
                return OsType.Windows;
            default:
                throw new NotImplementedException("Os not supported.");
        }
    }
}

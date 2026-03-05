using System;
using Wave.Domain.Java;
using Wave.Domain.Os;
using Wave.Infrastructure.Out.Java.Adoptium.Dtos;

namespace Wave.Infrastructure.Out.Java.Adoptium.Mappers;

public static class Mapper
{
    public static JavaVersion ToDomain(BuildsDto build, BinaryDto binary)
    {
        if (binary.Installer is null && binary.Package is null)
            throw new NullReferenceException("Installer and Package cannot be null at the same time.");

        List<JavaArtifact> artifacts = new List<JavaArtifact>();

        if (binary.Installer is not null)
        {
            artifacts.Add(new()
            {
                Type = JavaArtifactType.Installer,
                DownloadUrl = binary.Installer.DownloadUrl
            });
        }

        if (binary.Package is not null)
        {
            artifacts.Add(new()
            {
                Type = JavaArtifactType.Compressed,
                DownloadUrl = binary.Package.DownloadUrl
            });
        }


        return new JavaVersion()
        {
            Version = build.Version.Major,
            ArchitectureBitType = ToDomainArchitectureBitType(binary.Architecture),
            ArchitectureType = ToDomainArchitectureType(binary.Architecture),
            OsType = ToDomainOsType(binary.Os),
            JavaArtifacts = artifacts,
            Name = build.ReleaseName
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

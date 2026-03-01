using System;
using Wave.Application.Out.Java;
using Wave.Domain.Java;
using Wave.Domain.Os;

namespace Wave.Infrastructure.Out.Java.Microsoft;

public class MicrosoftJavaSupplier : IJavaSupplier
{
    //TODO: Mover a un JSON o similar
    private static readonly JavaVersion[] javaVersions =
    {
        // VERSION 25
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-linux-x64.tar.gz"
        },
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-macOS-x64.tar.gz"
        },
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-windows-x64.zip"
        },
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-windows-x64.msi"
        },
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-linux-aarch64.tar.gz"
        },
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-macOS-aarch64.tar.gz"
        },
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-windows-aarch64.zip"
        },
        new()
        {
            Version = 25,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-25-windows-aarch64.msi"
        },
        // VERSION 21
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-linux-x64.tar.gz"
        },
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-macOS-x64.tar.gz"
        },
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-windows-x64.zip"
        },
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-windows-x64.msi"
        },
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-linux-aarch64.tar.gz"
        },
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-macOS-aarch64.tar.gz"
        },
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-windows-aarch64.zip"
        },
        new()
        {
            Version = 21,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-21-windows-aarch64.msi"
        },
        // VERSION 17
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-linux-x64.tar.gz"
        },
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-macOS-x64.tar.gz"
        },
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-windows-x64.zip"
        },
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-windows-x64.msi"
        },
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-linux-aarch64.tar.gz"
        },
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-macOS-aarch64.tar.gz"
        },
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-windows-aarch64.zip"
        },
        new()
        {
            Version = 17,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-17-windows-aarch64.msi"
        },
        // VERSION 11
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-linux-x64.tar.gz"
        },
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-macOS-x64.tar.gz"
        },
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-windows-x64.zip"
        },
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.X86,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-windows-x64.msi"
        },
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Linux,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-linux-aarch64.tar.gz"
        },
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.MacOs,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-macOS-aarch64.tar.gz"
        },
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Compressed,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-windows-aarch64.zip"
        },
        new()
        {
            Version = 11,
            ArchitectureType = ArchitectureType.Arm,
            ArchitectureBitType = 64,
            OsType = OsType.Windows,
            FileType = FileType.Installer,
            DownloadUrl = "https://aka.ms/download-jdk/microsoft-jdk-11-windows-aarch64.msi"
        },
    };
    public async Task<IEnumerable<JavaVersion>> GetJavaVersionsAsync(JavaSupplierQuery? query, CancellationToken ct)
    {
        return javaVersions
    .Where(j => (query is null) ||
        (query.Version is null || j.Version == query.Version.Value) &&
        (query.OsType is null || j.OsType == query.OsType.Value) &&
        (query.ArchitectureType is null || j.ArchitectureType == query.ArchitectureType.Value) &&
        (query.ArchitectureBitType is null || j.ArchitectureBitType == query.ArchitectureBitType.Value)
    )
    .ToList();
    }
}

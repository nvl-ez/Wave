using Microsoft.Extensions.Logging;
using Wave.Application.Services;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Domain.Mods;
using Wave.Infrastructure.Out.Java;
using Wave.Infrastructure.Out.Java.Adoptium;
using Wave.Infrastructure.Out.Java.Installer;
using Wave.Infrastructure.Out.Java.Mojang;
using Wave.Infrastructure.Out.Java.Repository;
using Wave.Infrastructure.Out.Minecraft.Api;
using Wave.Infrastructure.Out.Modloader.Fabric.Api;
using Wave.Infrastructure.Out.Modloader.Forge.Api;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api;

namespace Wave.Ui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		AppComposition.Init();

		var mcsvc = new MinecraftVersionCatalogService(new MinecraftVersionCatalog());
		var fgsvc = new ModloaderVersionCatalogService(new ForgeVersionCatalog());
		var fasvc = new ModloaderVersionCatalogService(new FabricVersionCatalog());
		var cfsvc = new CurseforgeModSupplierIntegration();
		var mrsvc = new ModrinthModSupplierIntegration();
		var adsvc = new AdoptiumJavaSupplier();
		var mjsvc = new MojangJavaSupplier();
		var misvc = new ManifestInstaller("C:\\Users\\nahu\\Documents\\JavaTest");
		var cisvc = new CompressedInstaller("C:\\Users\\nahu\\Documents\\JavaTest");
		var eisvc = new ExecutableInstaller("C:\\Users\\nahu\\Documents\\JavaTest");
		var jrsvc = new JavaJsonRepository("C:\\Users\\nahu\\Documents\\JavaTest");
		var versions = Task.Run(async () =>
		{
			MinecraftVersion mc = new()
			{
				Version = "1.14",
				MinecraftVersionType = MinecraftVersionType.Release,
				ReleaseDate = DateTime.Now
			};
			ModSupplierQuery query = new ModSupplierQuery()
			{
				MinecraftVersion = mc,
				ModloaderType = ModloaderType.Forge,
			};
			JavaSupplierQuery jQuery = new JavaSupplierQuery()
			{
				OsType = Domain.Os.OsType.Windows,
				ArchitectureBitType = 64,
				ArchitectureType = Domain.Os.ArchitectureType.X86
			};
			JavaVersion javaVersion = new JavaVersion()
			{
				ArchitectureBitType = 64,
				ArchitectureType = Domain.Os.ArchitectureType.X86,
				JavaArtifacts = [
					new JavaArtifact(){
						//DownloadUrl = "https://piston-meta.mojang.com/v1/packages/b374544c680d965fb5535977d7cb04c6befe1930/manifest.json", //Windows Manifest
						DownloadUrl = "https://github.com/adoptium/temurin8-binaries/releases/download/jdk8u482-b08/OpenJDK8U-jre_x64_windows_hotspot_8u482b08.zip", //Windows zip
						//DownloadUrl = "https://github.com/adoptium/temurin8-binaries/releases/download/jdk8u482-b08/OpenJDK8U-jre_x64_linux_hotspot_8u482b08.tar.gz", //Linux gz
						//DownloadUrl = "https://github.com/adoptium/temurin8-binaries/releases/download/jdk8u472-b08/OpenJDK8U-jre_x64_windows_hotspot_8u472b08.msi", //Windows msi
						Type = JavaArtifactType.Compressed
					}
				],
				JavaSupplierType = JavaSupplierType.Adoptium,
				Name = "java-runtime-epsilon",
				OsType = Domain.Os.OsType.Windows,
				Version = 25,
			};
			//await mcsvc.GetMinecraftVersionsAsync(false, CancellationToken.None);
			//await fgsvc.GetModloaderVersionsAsync(mc, CancellationToken.None);
			//await fasvc.GetModloaderVersionsAsync(mc, CancellationToken.None);
			//var result = await cfsvc.SearchModsAsync(query, CancellationToken.None);
			//await cfsvc.GetModVersionsAsync(result.First(), CancellationToken.None);
			//var result = await mrsvc.SearchModsAsync(query, CancellationToken.None);
			//await mrsvc.GetModVersionsAsync(result.First(), CancellationToken.None);
			//await adsvc.GetJavaVersionsAsync(jQuery, CancellationToken.None);
			//await mjsvc.GetJavaVersionsAsync(jQuery, CancellationToken.None);
			//JavaInstallation? ji = await misvc.Install(javaVersion, javaVersion.JavaArtifacts.First(), CancellationToken.None);
			//await misvc.Uninstall(ji!, CancellationToken.None);
			JavaInstallation? ji = await cisvc.Install(javaVersion, javaVersion.JavaArtifacts.First(), CancellationToken.None);
			await jrsvc.AddAsync(ji!, CancellationToken.None);
			await cisvc.Uninstall(ji!, CancellationToken.None);
			await jrsvc.RemoveAsync(ji!, CancellationToken.None);
			//JavaInstallation? ji = await eisvc.Install(javaVersion, javaVersion.JavaArtifacts.First(), CancellationToken.None);
			//await eisvc.Uninstall(ji!, CancellationToken.None);
		});

		return builder.Build();
	}
}

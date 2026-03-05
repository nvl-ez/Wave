using Microsoft.Extensions.Logging;
using Wave.Application.Services;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Domain.Mods;
using Wave.Infrastructure.Out.Java;
using Wave.Infrastructure.Out.Java.Adoptium;
using Wave.Infrastructure.Out.Java.Mojang;
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
				OsType = Domain.Os.OsType.Windows
			};
			//await mcsvc.GetMinecraftVersionsAsync(false, CancellationToken.None);
			//await fgsvc.GetModloaderVersionsAsync(mc, CancellationToken.None);
			//await fasvc.GetModloaderVersionsAsync(mc, CancellationToken.None);
			//var result = await cfsvc.SearchModsAsync(query, CancellationToken.None);
			//await cfsvc.GetModVersionsAsync(result.First(), CancellationToken.None);
			//var result = await mrsvc.SearchModsAsync(query, CancellationToken.None);
			//await mrsvc.GetModVersionsAsync(result.First(), CancellationToken.None);
			//await adsvc.GetJavaVersionsAsync(jQuery, CancellationToken.None);
			await mjsvc.GetJavaVersionsAsync(jQuery, CancellationToken.None);
		});

		return builder.Build();
	}
}

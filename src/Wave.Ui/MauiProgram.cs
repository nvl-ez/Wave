using Microsoft.Extensions.Logging;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;
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
using Wave.Infrastructure.Out.ServerManager;
using CommunityToolkit.Maui;

namespace Wave.Ui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>().ConfigureFonts(fonts =>
		{
			fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
		}).UseMauiCommunityToolkit();
#if DEBUG
		builder.Logging.AddDebug();
#endif
		return builder.Build();
	}
}
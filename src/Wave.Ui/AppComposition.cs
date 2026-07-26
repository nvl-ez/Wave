using Wave.Application.Out.ServerManager;
using Wave.Infrastructure.Out.ServerManager;
using Wave.Application.Out.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Api;
using Wave.Infrastructure.Out.Minecraft.ServerProperties;
using Wave.Application.In;
using Wave.Infrastructure.In;
using Wave.Application.Out.Java;
using Wave.Infrastructure.Out.ServerManager.Executor;
using Wave.Infrastructure.Out.Java.Repository;
using Wave.Ui.Pages.ServersContent.ViewModels;
using Wave.Ui.Pages.ServerContent.ViewModels;
using Wave.Ui.Pages.SettingsContent.ViewModels;
using Wave.Infrastructure.Out.Java.Adoptium;
using Wave.Infrastructure.Out.Java.Mojang;
using Wave.Infrastructure.Out.Java.Installer;
using Wave.Application.Out.Platform;
using Wave.Infrastructure.Out.Platform;
using Wave.Ui.Pages.ExecutionContent.ViewModels;
using Wave.Application.Middle;
using Wave.Infrastructure.Middle;
using Wave.Application.Out.Modloader;
using Wave.Infrastructure.Out.Modloader.Forge.Api;
using Wave.Infrastructure.Out.Modloader.Fabric.Api;
using Wave.Application.Out.ModSupplier;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api;

namespace Wave.Ui;

public static class AppComposition
{
    //APP FOLDER NAME
    private const string serversDirectoryName = "Servers";
    private const string javaDirectoryName = "Java";
    private const string tmpDirectoryName = "tmp";


    // OUT PORTS
    private static readonly IServerRepository serverRepository;
    private static readonly IMinecraftVersionRepository minecraftVersionRepository;
    private static readonly IServerPropertyDefinitionRepository serverPropertyDefinitionRepository;
    private static readonly IServerPropertiesRepository serverPropertiesRepository;
    private static readonly IServerEulaRepository serverEulaRepository;
    private static readonly IServerExecutor serverExecutor;
    private static readonly IJavaInstallRepository javaInstallRepository;
    private static readonly IJavaSupplier adoptiumJavaSupplier;
    private static readonly IJavaSupplier mojangJavaSupplier;
    private static readonly IJavaInstaller compressedJavaInstaller;
    private static readonly IJavaInstaller manifestJavaInstaller;
    private static readonly IDeviceInformationRepository windowsDeviceInformationRepository;
    private static readonly IModloaderVersionCatalog forgeVersionCatalog;
    private static readonly IModloaderVersionCatalog fabricVersionCatalog;
    private static readonly IModSupplierIntegration curseforgeModSupplier;
    private static readonly IModSupplierIntegration modrinthModSupplier;

    //SERVICES
    private static readonly IMinecraftCatalogService minecraftCatalogService;
    private static readonly IServerManagerService serverHandlerService;
    private static readonly IServerExecutorService serverExecutorService;
    private static readonly IJavaManagerService javaManagerService;
    private static readonly IDeviceInformationService deviceInformationService;
    private static readonly IPropertiesManagerService propertiesManagerService;
    private static readonly IEulaManagerService eulaManagerService;
    private static readonly IVersionManagerService versionManagerService;
    private static readonly IModloaderManagerService modloaderManagerService;
    private static readonly IServerPathResolver serverPathResolver;
    private static readonly IModloaderCatalogService modloaderCatalogService;
    private static readonly IModCatalogService modCatalogService;
    private static readonly IModManagerService modManagerService;

    static AppComposition()
    {
        string appDirectory = FileSystem.AppDataDirectory;
        string serversDirectory = Path.Combine(appDirectory, serversDirectoryName);
        string javaDirectory = Path.Combine(appDirectory, javaDirectoryName);
        string tmpDirectory = Path.Combine(appDirectory, tmpDirectoryName);
        Directory.CreateDirectory(appDirectory);
        Directory.CreateDirectory(serversDirectory);
        Directory.CreateDirectory(javaDirectory);
        Directory.CreateDirectory(tmpDirectory);


        // OUT PORTS
        serverRepository = new JsonServerRepository(appDirectory);
        minecraftVersionRepository = new ApiMinecraftVersionRepository();
        serverPropertyDefinitionRepository = new InMemoryServerPropertyDefinitionRepository();
        serverPropertiesRepository = new ServerPropertiesRepository();
        serverExecutor = new WindowsServerExecutor(); //TODO: Add OSs
        javaInstallRepository = new JsonJavaRepository(javaDirectory);
        adoptiumJavaSupplier = new ApiAdoptiumJavaSupplier(tmpDirectory);
        mojangJavaSupplier = new ApiMojangJavaSupplier(tmpDirectory);
        compressedJavaInstaller = new CompressedJavaInstaller(javaDirectory);
        manifestJavaInstaller = new ManifestJavaInstaller(javaDirectory);
        windowsDeviceInformationRepository = new WindowsDeviceInformationRepository();
        serverEulaRepository = new ServerEulaRepository();
        forgeVersionCatalog = new ApiForgeVersionCatalog();
        fabricVersionCatalog = new ApiFabricVersionCatalog();
        curseforgeModSupplier = new ApiCurseforgeModSupplier();
        modrinthModSupplier = new ApiModrinthModSupplier();



        //SERVICES
        serverPathResolver = new ServerPathResolver(appDirectory, serversDirectory, tmpDirectory);
        propertiesManagerService = new PropertiesManagerService(serverPathResolver, serverPropertiesRepository);
        eulaManagerService = new EulaManagerService(serverPathResolver, serverEulaRepository);
        versionManagerService = new VersionManagerService(serverPathResolver, minecraftVersionRepository);
        modloaderManagerService = new ModloaderManagerService(serverPathResolver, [forgeVersionCatalog, fabricVersionCatalog], javaInstallRepository);
        modloaderCatalogService = new ModloaderCatalogService([forgeVersionCatalog, fabricVersionCatalog]);
        modCatalogService = new ModCatalogService([curseforgeModSupplier, modrinthModSupplier]);
        modManagerService = new ModManagerService(serverPathResolver, [curseforgeModSupplier, modrinthModSupplier]);


        minecraftCatalogService = new MinecraftCatalogService(minecraftVersionRepository, serverPropertyDefinitionRepository);
        serverHandlerService = new ServerManagerService(serverPathResolver, serverRepository, versionManagerService, propertiesManagerService, eulaManagerService, modloaderManagerService, modManagerService);
        serverExecutorService = new ServerExecutorService(serverPathResolver, serverExecutor, serverRepository, javaInstallRepository);
        javaManagerService = new JavaManagerService(javaInstallRepository, [adoptiumJavaSupplier, mojangJavaSupplier], [compressedJavaInstaller, manifestJavaInstaller]);
        deviceInformationService = new DeviceInformationService(windowsDeviceInformationRepository);
    }

    // VIEW MODELS
    public static ServersViewModel CreateServersViewModel() => new ServersViewModel(serverHandlerService, serverExecutorService);
    public static ServerViewModel CreateServerViewModel() => new ServerViewModel(minecraftCatalogService, serverHandlerService, modloaderCatalogService, modCatalogService);
    public static ExecutionViewModel CreateExecutionViewModel() => new ExecutionViewModel(serverExecutorService);
    public static JavaViewModel CreateJavaViewModel() => new JavaViewModel(deviceInformationService, javaManagerService);
    public static SettingsViewModel CreateSettingsViewModel() => new SettingsViewModel();

    public static IServerExecutorService GetServerExecutorService() => serverExecutorService;
}

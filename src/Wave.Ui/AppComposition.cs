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

namespace Wave.Ui;

public static class AppComposition
{
    //APP FOLDER NAME
    private const string serversDirectoryName = "Servers";
    private const string javaDirectoryName = "Java";
    private const string tmpDirectoryName = "tmp";


    // OUT PORTS
    private static IServerRepository serverRepository;
    private static IMinecraftVersionRepository minecraftVersionRepository;
    private static IServerPropertyDefinitionRepository serverPropertyDefinitionRepository;
    private static IServerPropertiesRepository serverPropertiesRepository;
    private static IServerEulaRepository serverEulaRepository;
    private static IServerExecutor serverExecutor;
    private static IJavaInstallRepository javaInstallRepository;
    private static IJavaSupplier adoptiumJavaSupplier;
    private static IJavaSupplier mojangJavaSupplier;
    private static IJavaInstaller compressedJavaInstaller;
    private static IJavaInstaller manifestJavaInstaller;
    private static IDeviceInformationRepository windowsDeviceInformationRepository;

    //SERVICES
    private static IMinecraftCatalogService minecraftCatalogService;
    private static IServerManagerService serverHandlerService;
    private static IServerExecutorService serverExecutorService;
    private static IJavaManagerService javaManagerService;
    private static IDeviceInformationService deviceInformationService;
    private static IPropertiesManagerService propertiesManagerService;
    private static IEulaManagerService eulaManagerService;
    private static IVersionManagerService versionManagerService;
    private static IModloaderManagerService modloaderManagerService;
    private static IServerPathResolver serverPathResolver;

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


        //SERVICES
        serverPathResolver = new ServerPathResolver(appDirectory, serversDirectory, tmpDirectory);
        propertiesManagerService = new PropertiesManagerService(serverPathResolver, serverPropertiesRepository);
        eulaManagerService = new EulaManagerService(serverPathResolver, serverEulaRepository);
        versionManagerService = new VersionManagerService(serverPathResolver, minecraftVersionRepository);
        modloaderManagerService = new ModloaderManagerService(serverPathResolver, [], javaInstallRepository);


        minecraftCatalogService = new MinecraftCatalogService(minecraftVersionRepository, serverPropertyDefinitionRepository);
        serverHandlerService = new ServerManagerService(serverPathResolver, serverRepository, versionManagerService, propertiesManagerService, eulaManagerService, modloaderManagerService);
        serverExecutorService = new ServerExecutorService(serverPathResolver, serverExecutor, serverRepository, javaInstallRepository);
        javaManagerService = new JavaManagerService(javaInstallRepository, [adoptiumJavaSupplier, mojangJavaSupplier], [compressedJavaInstaller, manifestJavaInstaller]);
        deviceInformationService = new DeviceInformationService(windowsDeviceInformationRepository);
    }

    // VIEW MODELS
    public static ServersViewModel CreateServersViewModel() => new ServersViewModel(serverHandlerService, serverExecutorService);
    public static ServerViewModel CreateServerViewModel() => new ServerViewModel(minecraftCatalogService, serverHandlerService);
    public static ExecutionViewModel CreateExecutionViewModel() => new ExecutionViewModel(serverExecutorService);
    public static JavaViewModel CreateJavaViewModel() => new JavaViewModel(deviceInformationService, javaManagerService);
    public static SettingsViewModel CreateSettingsViewModel() => new SettingsViewModel();
}

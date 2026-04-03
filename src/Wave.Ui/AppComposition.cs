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

namespace Wave.Ui;

public static class AppComposition
{
    //APP FOLDER NAME
    private const string serverDirectoryName = "Servers";
    private const string javaDirectoryName = "Java";


    // OUT PORTS
    private static IServerRepository serverRepository;
    private static IMinecraftVersionRepository minecraftVersionRepository;
    private static IServerPropertyDefinitionRepository serverPropertyDefinitionRepository;
    private static IServerPropertiesRepository serverPropertiesRepository;
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

    static AppComposition()
    {
        string appDirectory = FileSystem.AppDataDirectory;
        string serverDirectory = Path.Combine(appDirectory, serverDirectoryName);
        string javaDirectory = Path.Combine(appDirectory, javaDirectoryName);
        string javaTmpDirectory = Path.Combine(javaDirectory, "tmp");
        Directory.CreateDirectory(appDirectory);
        Directory.CreateDirectory(serverDirectory);
        Directory.CreateDirectory(javaDirectory);
        Directory.CreateDirectory(javaTmpDirectory);


        // OUT PORTS
        serverRepository = new JsonServerRepository(appDirectory);
        minecraftVersionRepository = new ApiMinecraftVersionRepository();
        serverPropertyDefinitionRepository = new InMemoryServerPropertyDefinitionRepository();
        serverPropertiesRepository = new ServerPropertiesRepository();
        serverExecutor = new WindowsServerExecutor(); //TODO: Add OSs
        javaInstallRepository = new JsonJavaRepository(javaDirectory);
        adoptiumJavaSupplier = new ApiAdoptiumJavaSupplier(javaTmpDirectory);
        mojangJavaSupplier = new ApiMojangJavaSupplier(javaTmpDirectory);
        compressedJavaInstaller = new CompressedJavaInstaller(javaDirectory);
        manifestJavaInstaller = new ManifestJavaInstaller(javaDirectory);
        windowsDeviceInformationRepository = new WindowsDeviceInformationRepository();


        //SERVICES
        minecraftCatalogService = new MinecraftCatalogService(minecraftVersionRepository, serverPropertyDefinitionRepository);
        serverHandlerService = new ServerManagerService(serverDirectory, serverRepository, minecraftVersionRepository, serverPropertiesRepository);
        serverExecutorService = new ServerExecutorService(serverExecutor, serverRepository, javaInstallRepository);
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

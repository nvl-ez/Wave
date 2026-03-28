using System;
using Wave.Application.Out;
using Wave.Domain;
using Wave.Infrastructure.Out;
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
using Wave.Ui.Pages.ExecutionContent;

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

    //SERVICES
    private static IMinecraftCatalogService minecraftCatalogService;
    private static IServerManagerService serverHandlerService;
    private static IServerExecutorService serverExecutorService;

    static AppComposition()
    {
        string appDirectory = FileSystem.AppDataDirectory;
        string serverDirectory = Path.Combine(appDirectory, serverDirectoryName);
        string javaDirectory = Path.Combine(appDirectory, javaDirectoryName);
        Directory.CreateDirectory(appDirectory);
        Directory.CreateDirectory(serverDirectory);
        Directory.CreateDirectory(javaDirectory);


        // OUT PORTS
        serverRepository = new JsonServerRepository(appDirectory);
        minecraftVersionRepository = new ApiMinecraftVersionRepository();
        serverPropertyDefinitionRepository = new InMemoryServerPropertyDefinitionRepository();
        serverPropertiesRepository = new ServerPropertiesRepository();
        serverExecutor = new WindowsServerExecutor(); //TODO: Add OSs
        javaInstallRepository = new JsonJavaRepository(javaDirectory);


        //SERVICES
        minecraftCatalogService = new MinecraftCatalogService(minecraftVersionRepository, serverPropertyDefinitionRepository);
        serverHandlerService = new ServerManagerService(serverDirectory, serverRepository, minecraftVersionRepository, serverPropertiesRepository);
        serverExecutorService = new ServerExecutorService(serverExecutor, serverRepository, javaInstallRepository);
    }

    // VIEW MODELS
    public static ServersViewModel CreateServersViewModel() => new ServersViewModel(serverHandlerService, serverExecutorService);
    public static ServerViewModel CreateServerViewModel() => new ServerViewModel(minecraftCatalogService, serverHandlerService);
    public static ExecutionViewModel CreateExecutionViewModel() => new ExecutionViewModel(serverExecutorService);

}

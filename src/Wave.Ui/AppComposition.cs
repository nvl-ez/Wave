using System;
using Wave.Application.Out;
using Wave.Domain;
using Wave.Infrastructure.Out;
using Wave.Application.Out.ServerManager;
using Wave.Ui.ViewModels;
using Wave.Infrastructure.Out.ServerManager;
using Wave.Application.Out.Minecraft;
using Wave.Infrastructure.Out.Minecraft.Api;
using Wave.Infrastructure.Out.Minecraft.ServerProperties;
using Wave.Application.In;
using Wave.Infrastructure.In;

namespace Wave.Ui;

public static class AppComposition
{
    //APP FOLDER NAME
    private const string serversDirectoryName = "Servers";
    private static INoteRepository noteRepository;

    // OUT PORTS
    private static IServerRepository serverRepository;
    private static IMinecraftVersionRepository minecraftVersionRepository;
    private static IServerPropertyDefinitionRepository serverPropertyDefinitionRepository;
    private static IServerPropertiesRepository serverPropertiesRepository;

    //SERVICES
    private static IMinecraftCatalogService minecraftCatalogService;
    private static IServerManagerService serverHandlerService;

    static AppComposition()
    {
        string appDirectory = FileSystem.AppDataDirectory;
        string serversDirectory = Path.Combine(appDirectory, serversDirectoryName);
        Directory.CreateDirectory(appDirectory);
        Directory.CreateDirectory(serversDirectory);


        noteRepository = new NoteRepository(appDirectory);

        // OUT PORTS
        serverRepository = new JsonServerRepository(appDirectory);
        minecraftVersionRepository = new ApiMinecraftVersionRepository();
        serverPropertyDefinitionRepository = new InMemoryServerPropertyDefinitionRepository();
        serverPropertiesRepository = new ServerPropertiesRepository();

        //SERVICES
        minecraftCatalogService = new MinecraftCatalogService(minecraftVersionRepository, serverPropertyDefinitionRepository);
        serverHandlerService = new ServerManagerService(serversDirectory, serverRepository, minecraftVersionRepository, serverPropertiesRepository);
    }

    public static NotesViewModel CreateNotesViewModel() => new NotesViewModel(noteRepository);
    public static NoteViewModel CreateNoteViewModel() => new NoteViewModel(noteRepository);
    public static NoteViewModel CreateNoteViewModel(Note note) => new NoteViewModel(noteRepository, note);

    // VIEW MODELS
    public static ServersViewModel CreateServersViewModel() => new ServersViewModel(serverHandlerService);
    public static ServerViewModel CreateServerViewModel() => new ServerViewModel(minecraftCatalogService, serverHandlerService);

}

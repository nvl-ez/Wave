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
    private const string directoryName = ".Wave";
    private static INoteRepository noteRepository;

    // OUT PORTS
    private static IServerRepository serverRepository;
    private static IMinecraftVersionRepository minecraftVersionRepository;
    private static IServerPropertiesRepository serverPropertiesRepository;

    //SERVICES
    private static IMinecraftCatalogService minecraftCatalogService;
    private static IServerCatalogService serverCatalogService;
    public static void Init()
    {
        string appDataDirectory = FileSystem.AppDataDirectory;
        string appDirectory = Path.Combine(appDataDirectory, directoryName);
        Directory.CreateDirectory(appDirectory);


        noteRepository = new NoteRepository(appDataDirectory);

        // OUT PORTS
        serverRepository = new JsonServerRepository(appDirectory);
        minecraftVersionRepository = new ApiMinecraftVersionRepository();
        serverPropertiesRepository = new InMemoryServerPropertiesRepository();

        //SERVICES
        minecraftCatalogService = new MinecraftCatalogService(minecraftVersionRepository, serverPropertiesRepository);
        serverCatalogService = new ServerCatalogService(serverRepository);
    }

    public static NotesViewModel CreateNotesViewModel() => new NotesViewModel(noteRepository);
    public static NoteViewModel CreateNoteViewModel() => new NoteViewModel(noteRepository);
    public static NoteViewModel CreateNoteViewModel(Note note) => new NoteViewModel(noteRepository, note);

    // VIEW MODELS
    public static ServersViewModel CreateServersViewModel() => new ServersViewModel(serverCatalogService);
    public static ServerViewModel CreateServerViewModel() => new ServerViewModel(serverCatalogService, minecraftCatalogService);

}

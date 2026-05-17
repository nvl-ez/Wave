using Wave.Application.Middle;
using Wave.Application.Out.Java;
using Wave.Application.Out.Modloader;
using Wave.Domain.Java;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Infrastructure.Middle;

public class ModloaderManagerService : IModloaderManagerService
{
    private readonly IServerPathResolver serverPathResolver;
    private readonly IEnumerable<IModloaderVersionCatalog> modloaders;
    private readonly IJavaInstallRepository javaInstallRepository;

    public ModloaderManagerService(IServerPathResolver serverPathResolver, IEnumerable<IModloaderVersionCatalog> modloaders, IJavaInstallRepository javaInstallRepository)
    {
        this.modloaders = modloaders;
        this.serverPathResolver = serverPathResolver;
        this.javaInstallRepository = javaInstallRepository;
    }

    public async Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(ModloaderType modloaderType, MinecraftVersionInfo minecraftVersionInfo, CancellationToken ct = default)
    {
        IModloaderVersionCatalog? targetModloader = null;
        foreach (var modloader in modloaders)
        {
            if (modloader.CanHandleType(modloaderType))
            {
                targetModloader = modloader;
                break;
            }
        }

        if (targetModloader is null) throw new InvalidDataException($"There is no modloader that can handle the typ {modloaderType}.");//TODO: Fix error handling

        return await targetModloader.GetModloaderVersionsAsync(minecraftVersionInfo);
    }

    public async Task<Server> AddModloaderAsync(Server server, ModloaderInfo modloaderInfo, CancellationToken ct = default)
    {
        if (server.Modloader is not null) throw new InvalidDataException($"The server already has an installed modloader.");

        IModloaderVersionCatalog? target = null;
        foreach (var modloader in modloaders)
        {
            if (modloader.CanHandleType(modloaderInfo.ModloaderType)) target = modloader;
        }

        if (target is null) throw new InvalidDataException($"The type {modloaderInfo.ModloaderType} is not supported by any modloader.");

        string filePath = Path.Combine(serverPathResolver.GetTmpDirectory(), "modloader.jar");

        //Obtener el paquete
        ModloaderPackage modloaderPackage = await target.DownloadModloaderAsync(modloaderInfo, filePath);

        //Obtener la verion de java mas nueva
        JavaInstallation javaInstallation = (await javaInstallRepository.GetAllAsync()).OrderByDescending(j => j.Version).First();

        server.Modloader = await target.InstallModloaderAsync(
            serverPathResolver.GetServerRootDirectory(server),
            modloaderPackage,
            javaInstallation
            );

        //Clear tmp folder
        string[] allFiles = Directory.GetFiles(serverPathResolver.GetTmpDirectory());
        foreach (string file in allFiles)
        {
            if (new[] { "forge", "fabric", "modloader" }.Any(c => Path.GetFileName(file).ToLower().Contains(c)))
            {
                File.Delete(file);
            }
        }

        return server;
    }

    public async Task<Server> RemoveModloaderAsync(Server server, CancellationToken ct = default)
    {
        if (server.Modloader is null) throw new InvalidDataException($"The server does not have an installed modloader.");

        string serverDirectory = serverPathResolver.GetServerRootDirectory(server);

        //Obtain all directories in the server folder
        var directories = Directory.GetDirectories(serverDirectory, "*", SearchOption.AllDirectories);
        foreach (var directory in directories)
        {
            if (new[] { "forge", "fabric", "modloader" }.Any(c => Path.GetDirectoryName(directory)!.ToLower().Contains(c)))
            {
                Directory.Delete(directory, true);
            }
        }

        //Obtain all files in the server folder
        var files = Directory.GetFiles(serverPathResolver.GetTmpDirectory(), "*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (new[] { "forge", "fabric", "modloader" }.Any(c => Path.GetFileName(file).ToLower().Contains(c)))
            {
                File.Delete(file);
            }
        }

        server.Modloader = null;
        return server;
    }
}

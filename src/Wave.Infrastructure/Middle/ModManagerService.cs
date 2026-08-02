using System;
using Wave.Application.Middle;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;
using System.Collections.Generic;

namespace Wave.Infrastructure.Middle;

public class ModManagerService : IModManagerService
{
    private readonly IModSupplierIntegration[] modSupplierIntegrations;
    private readonly IServerPathResolver serverPathResolver;

    public ModManagerService(IServerPathResolver serverPathResolver, IModSupplierIntegration[] modSupplierIntegrations)
    {
        this.modSupplierIntegrations = modSupplierIntegrations;
        this.serverPathResolver = serverPathResolver;
    }



    public async Task<IEnumerable<ModFile>> SetModsAsync(Server server, ServerQuery query)
    {
        //Differing de mods por id de mod y version
        var added = query.Mods.ExceptBy(
                            server.Mods.Select(x => (x.ModId, x.VersionId)),
                            x => (x.ModId, x.VersionId))
                        .ToList();

        var removed = server.Mods.ExceptBy(
                query.Mods.Select(x => (x.ModId, x.VersionId)),
                x => (x.ModId, x.VersionId))
            .ToList();

        var failedMods = await AddModsAsync(server, added);
        await RemoveModsAsync(server, removed);

        return failedMods;
    }

    public async Task<ModMigrationResult> MigrateModsAsync(Server server)
    {
        List<ModFile> removedMods = new();
        List<ModFile> failedMods = new();

        string modsDirectory = serverPathResolver.GetModsDirectory(server);

        string serverMinecraftVersion = server.MinecraftVersionInstallation!.MinecraftVersion;
        foreach (var mod in server.Mods)
        {
            if (mod.MinecraftVersion == serverMinecraftVersion && mod.ModloaderType == server.Modloader?.ModloaderType) continue;

            //Uninstall and unlist mod
            foreach (var modArtifact in mod.Artifacts)
            {
                string modFileName = Path.Combine(modsDirectory, modArtifact.FileName);
                UninstallMod(modFileName);
            }
            server.Mods = server.Mods.Where(m => m.ModId != mod.ModId);

            //Search for new versions
            IModSupplierIntegration? target = null;
            foreach (var modSupplier in modSupplierIntegrations)
            {
                if (modSupplier.CanHandle(mod.ModSupplierType))
                {
                    target = modSupplier;
                    break;
                }
            }
            if (target is null) throw new InvalidDataException($"There is no supported mod supplier of type {mod.ModSupplierType}");

            ModVersionSupplierQuery query = new()
            {
                MinecraftVersion = serverMinecraftVersion,
                ModId = mod.ModId,
                ModloaderType = server.Modloader!.ModloaderType,
                ModSupplierType = mod.ModSupplierType
            };

            var versions = (await target.GetModVersionsAsync(query)).Versions;

            // If no alternative return
            if (versions is null || versions.Count() == 0)
            {
                server.Mods = server.Mods.Where(m => m.ModId != mod.ModId);
                removedMods.Add(mod);
                continue;
            }

            //Download the latest version
            ModFile modFile = new(mod, versions.First());

            if (!await TryDownloadMod(modFile, modsDirectory))
            {
                failedMods.Add(modFile);
                continue;
            }

            server.Mods = server.Mods.Append(modFile);
        }

        return new ModMigrationResult
        {
            DeletedMods = removedMods,
            FailedMods = failedMods
        };
    }

    private async Task<IEnumerable<ModFile>> AddModsAsync(Server server, IEnumerable<ModFile> mods)
    {
        string modsDirectory = serverPathResolver.CreateModsDirectory(server);
        List<ModFile> failedMods = new();

        foreach (ModFile mod in mods)
        {
            if (mod is null) continue;

            if (!await TryDownloadMod(mod, modsDirectory))
            {
                failedMods.Add(mod);
                continue;
            }

            server.Mods = server.Mods.Append(mod);
        }

        return failedMods;
    }

    private async Task RemoveModsAsync(Server server, IEnumerable<ModFile> mods)
    {
        string modsDirectory = serverPathResolver.GetModsDirectory(server);
        if (!Directory.Exists(modsDirectory)) return;

        foreach (var mod in mods)
        {
            foreach (var modArtifact in mod.Artifacts)
            {
                string modFile = Path.Combine(modsDirectory, modArtifact.FileName);

                UninstallMod(modFile);

            }
            server.Mods = server.Mods.Where(m => m.ModId != mod.ModId);
        }
    }

    private void UninstallMod(string modFIle)
    {
        if (File.Exists(modFIle)) File.Delete(modFIle);
    }

    private async Task DownloadMod(ModFile mod, string modsDirectory)
    {
        //Find target mod supplier
        IModSupplierIntegration? target = null;
        foreach (var modSupplier in modSupplierIntegrations)
        {
            if (modSupplier.CanHandle(mod.ModSupplierType))
            {
                target = modSupplier;
                break;
            }
        }
        if (target is null) throw new InvalidDataException($"There is no supported mod supplier of type {mod.ModSupplierType}");

        //Download and store the mod
        await target.DownloadMod(mod, modsDirectory);
    }

    private async Task<bool> TryDownloadMod(ModFile mod, string modsDirectory)
    {
        try
        {
            await DownloadMod(mod, modsDirectory);
            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Could not download mod {mod.ModId}: {exception.Message}");

            foreach (var artifact in mod.Artifacts)
            {
                string artifactPath = Path.Combine(modsDirectory, artifact.FileName);
                try
                {
                    if (File.Exists(artifactPath))
                        File.Delete(artifactPath);
                }
                catch (IOException cleanupException)
                {
                    Console.Error.WriteLine($"Could not remove partial mod file {artifactPath}: {cleanupException.Message}");
                }
                catch (UnauthorizedAccessException cleanupException)
                {
                    Console.Error.WriteLine($"Could not remove partial mod file {artifactPath}: {cleanupException.Message}");
                }
            }

            return false;
        }
    }
}

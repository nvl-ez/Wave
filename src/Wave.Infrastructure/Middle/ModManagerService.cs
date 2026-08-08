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



    public async Task<ModMigrationResult> SetModsAsync(Server server, ServerQuery query)
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

        var result = await AddModsAsync(server, added, query.Mods);
        await RemoveModsAsync(server, removed);

        return result;
    }

    public async Task<ModMigrationResult> MigrateModsAsync(Server server)
    {
        List<ModFile> removedMods = new();
        List<ModFile> failedMods = new();
        List<ModFile> incompatibleMods = new();
        List<ModFile> requiredMods = new();

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

            var addResult = await AddModsAsync(server, [modFile], [modFile]);
            failedMods.AddRange(addResult.FailedMods);
            incompatibleMods.AddRange(addResult.IncompatibleMods);
            requiredMods.AddRange(addResult.RequiredMods);
        }

        return new ModMigrationResult
        {
            DeletedMods = removedMods,
            FailedMods = failedMods,
            IncompatibleMods = incompatibleMods,
            RequiredMods = requiredMods
        };
    }

    private async Task<ModMigrationResult> AddModsAsync(
        Server server,
        IEnumerable<ModFile> mods,
        IEnumerable<ModFile> knownMods)
    {
        string modsDirectory = serverPathResolver.CreateModsDirectory(server);
        List<ModFile> failedMods = new();
        List<ModFile> incompatibleMods = new();
        List<ModFile> requiredMods = new();
        var knownById = knownMods.GroupBy(mod => mod.ModId).ToDictionary(group => group.Key, group => group.First());
        var explicitlyRequestedIds = knownMods.Select(mod => mod.ModId).ToHashSet();

        foreach (ModFile mod in mods)
        {
            if (mod is null) continue;
            if (server.Mods.Any(existing => existing.ModId == mod.ModId && existing.VersionId == mod.VersionId))
                continue;

            var dependencyMods = new List<ModFile>();
            var resolution = await ResolveDependenciesAsync(
                mod,
                server,
                knownById,
                dependencyMods,
                new HashSet<string>());

            if (resolution == DependencyResolution.Incompatible)
            {
                incompatibleMods.Add(mod);
                continue;
            }
            if (resolution == DependencyResolution.Failed)
            {
                failedMods.Add(mod);
                continue;
            }

            bool dependencyDownloadFailed = false;
            foreach (var dependencyMod in dependencyMods)
            {
                if (server.Mods.Any(existing => existing.ModId == dependencyMod.ModId)) continue;

                if (!await TryDownloadMod(dependencyMod, modsDirectory))
                {
                    dependencyDownloadFailed = true;
                    break;
                }

                server.Mods = server.Mods.Append(dependencyMod);
                if (!explicitlyRequestedIds.Contains(dependencyMod.ModId))
                    requiredMods.Add(dependencyMod);
            }

            if (dependencyDownloadFailed || !await TryDownloadMod(mod, modsDirectory))
            {
                failedMods.Add(mod);
                continue;
            }

            server.Mods = server.Mods.Append(mod);
        }

        return new ModMigrationResult
        {
            FailedMods = failedMods,
            IncompatibleMods = incompatibleMods,
            RequiredMods = requiredMods
        };
    }

    private async Task<DependencyResolution> ResolveDependenciesAsync(
        ModFile mod,
        Server server,
        IReadOnlyDictionary<string, ModFile> knownMods,
        List<ModFile> dependencyMods,
        HashSet<string> visiting)
    {
        var installedAndPlanned = server.Mods.Concat(dependencyMods).ToList();
        if (IsIncompatibleWith(mod, installedAndPlanned))
            return DependencyResolution.Incompatible;

        if (!visiting.Add(mod.ModId))
            return DependencyResolution.Success;

        try
        {
            foreach (var dependency in mod.Dependencies.Where(dependency => dependency.DependencyType == ModDependencyType.Required))
            {
                if (server.Mods.Any(existing => existing.ModId == dependency.ModId) ||
                    dependencyMods.Any(existing => existing.ModId == dependency.ModId))
                    continue;

                ModFile? dependencyMod = await GetDependencyModAsync(mod, dependency, knownMods);
                if (dependencyMod is null)
                    return DependencyResolution.Failed;
                if (IsIncompatibleWith(mod, [dependencyMod]))
                    return DependencyResolution.Incompatible;

                var result = await ResolveDependenciesAsync(
                    dependencyMod,
                    server,
                    knownMods,
                    dependencyMods,
                    visiting);
                if (result != DependencyResolution.Success)
                    return result;

                if (!server.Mods.Any(existing => existing.ModId == dependencyMod.ModId) &&
                    !dependencyMods.Any(existing => existing.ModId == dependencyMod.ModId))
                    dependencyMods.Add(dependencyMod);
            }

            return DependencyResolution.Success;
        }
        finally
        {
            visiting.Remove(mod.ModId);
        }
    }

    private async Task<ModFile?> GetDependencyModAsync(
        ModFile parent,
        ModDependency dependency,
        IReadOnlyDictionary<string, ModFile> knownMods)
    {
        if (knownMods.TryGetValue(dependency.ModId, out var knownMod) &&
            (dependency.VersionId is null || knownMod.VersionId == dependency.VersionId))
            return knownMod;

        var target = modSupplierIntegrations.FirstOrDefault(supplier => supplier.CanHandle(parent.ModSupplierType));
        if (target is null)
            return null;

        var query = new ModVersionSupplierQuery
        {
            MinecraftVersion = parent.MinecraftVersion,
            ModId = dependency.ModId,
            ModloaderType = parent.ModloaderType,
            ModSupplierType = parent.ModSupplierType
        };
        var versions = (await target.GetModVersionsAsync(query)).Versions;
        var version = dependency.VersionId is null
            ? versions.FirstOrDefault()
            : versions.FirstOrDefault(candidate => candidate.VersionId == dependency.VersionId);
        if (version is null)
            return null;

        try
        {
            var info = await target.GetModInfoAsync(dependency.ModId);
            if (string.IsNullOrWhiteSpace(info.ModId) || string.IsNullOrWhiteSpace(info.ModName))
                return null;

            return new ModFile(info, version);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Could not retrieve metadata for required mod {dependency.ModId}: {exception.Message}");
            return null;
        }
    }

    private static bool IsIncompatibleWith(ModFile candidate, IEnumerable<ModFile> otherMods)
    {
        foreach (var other in otherMods.Where(other => other.ModId != candidate.ModId))
        {
            if (candidate.Dependencies.Any(dependency =>
                    dependency.DependencyType == ModDependencyType.Incompatible &&
                    dependency.ModId == other.ModId) ||
                other.Dependencies.Any(dependency =>
                    dependency.DependencyType == ModDependencyType.Incompatible &&
                    dependency.ModId == candidate.ModId))
                return true;
        }

        return false;
    }

    private enum DependencyResolution
    {
        Success,
        Failed,
        Incompatible
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

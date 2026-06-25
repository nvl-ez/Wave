using System;
using Wave.Application.Middle;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager;

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

    public async Task SetModsAsync(Server server, ServerQuery query)
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

        await AddModsAsync(server, added);
        await RemoveModsAsync(server, removed);
    }

    private async Task AddModsAsync(Server server, IEnumerable<ModFile> mods)
    {
        string modsDirectory = serverPathResolver.CreateModsDirectory(server);

        foreach (ModFile mod in mods)
        {
            if (mod is null) continue;

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

            server.Mods = server.Mods.Append(mod);
        }
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

                if (File.Exists(modFile)) File.Delete(modFile);
            }
        }
    }
}

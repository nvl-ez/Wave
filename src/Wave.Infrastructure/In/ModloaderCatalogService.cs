using System;
using System.Globalization;
using Wave.Application.In;
using Wave.Application.Out.Modloader;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Infrastructure.In;

public class ModloaderCatalogService : IModloaderCatalogService
{
    private readonly IEnumerable<IModloaderVersionCatalog> modloaders;

    public ModloaderCatalogService(IEnumerable<IModloaderVersionCatalog> modloaders)
    {
        this.modloaders = modloaders;
    }

    public async Task<IEnumerable<KeyValuePair<string, ModloaderType>>> GetModloaderTypesAsync(CancellationToken ct = default)
    {
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        return modloaders.Select(
            m => new KeyValuePair<string, ModloaderType>(ti.ToTitleCase(m.ModloaderType.ToString()), m.ModloaderType)
        );
    }

    public async Task<IEnumerable<ModloaderInfo>> GetModloaderVersionsAsync(ModloaderType modloaderType, string minecraftVersion, CancellationToken ct = default)
    {
        IModloaderVersionCatalog? target = null;

        foreach (var modloader in modloaders)
        {
            if (modloader.CanHandleType(modloaderType))
            {
                target = modloader;
                break;
            }
        }

        if (target is null) throw new InvalidDataException($"There is no modloader that can handle the type {modloaderType}.");

        return await target.GetModloaderVersionsAsync(minecraftVersion);
    }
}

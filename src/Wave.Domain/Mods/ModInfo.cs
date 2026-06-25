using System;
using Wave.Domain.Minecraft;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Domain.Mods;

public class ModInfo : ModBase
{
    public ModInfo(string modId, string name, string slug, ModSupplierType modSupplierType, string summary, string? iconUrl = null) : base(modId, name, slug, modSupplierType, summary, iconUrl)
    { }
}

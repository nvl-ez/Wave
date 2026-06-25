using System;

namespace Wave.Domain.Mods;

public class ModDetails : ModBase
{
    public ModDetails(ModBase modBase, string modDescription, ModDescriptionType modDescriptionType) : base(modBase.ModId, modBase.ModName, modBase.Slug, modBase.ModSupplierType, modBase.ModSummary, modBase.IconUrl)
    {
        ModDescription = modDescription;
        ModDescriptionType = modDescriptionType;
    }
    public string ModDescription { get; set; }
    public ModDescriptionType ModDescriptionType { get; set; }
}

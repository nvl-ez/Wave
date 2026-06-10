using System;

namespace Wave.Domain.Mods;

public class ModDetails : ModBase
{
    public required string ModDescription { get; set; }
    public required ModDescriptionType ModDescriptionType { get; set; }
}

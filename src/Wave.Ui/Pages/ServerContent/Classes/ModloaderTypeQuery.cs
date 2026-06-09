using System;
using System.Runtime.CompilerServices;
using Wave.Domain.ServerManager.Modloader;

namespace Wave.Ui.Pages.ServerContent.Classes;

public class ModloaderTypeQuery
{
    public ModloaderType? ModloaderType { get; set; } = null;

    public bool Equals(ModloaderTypeQuery? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        return ModloaderType == other.ModloaderType;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ModloaderTypeQuery);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ModloaderType);
    }
}

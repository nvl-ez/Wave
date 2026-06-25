using System;

namespace Wave.Domain.Mods;

public abstract class ModBase : IEquatable<ModBase>
{
    protected ModBase()
    {
        ModId = string.Empty;
        ModName = string.Empty;
        Slug = string.Empty;
        ModSummary = string.Empty;
        IconUrl = string.Empty;
    }
    public ModBase(string modId, string name, string slug, ModSupplierType modSupplierType, string summary, string? iconUrl = null)
    {
        ModId = modId;
        ModName = name;
        Slug = slug;
        ModSupplierType = modSupplierType;
        ModSummary = summary;
        IconUrl = iconUrl;
    }
    public ModSupplierType ModSupplierType { get; set; }
    public string ModName { get; set; }
    public string ModSummary { get; set; }
    public string? IconUrl { get; set; }
    public string ModId { get; set; }
    public string Slug { get; set; }

    public virtual bool Equals(ModBase? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        // Prevent ModBase and ModFile from being equal using different rules
        if (GetType() != other.GetType()) return false;

        return ModId == other.ModId;
    }

    public override bool Equals(object? obj)
    {
        return obj is ModBase other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), ModId);
    }
}

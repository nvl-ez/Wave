using System;

namespace Wave.Domain.Mods;

public abstract class ModBase : IEquatable<ModBase>
{
    public required ModSupplierType ModSupplierType { get; set; }
    public required string Name { get; set; }
    public required string Summary { get; set; }
    public string? IconUrl { get; set; }
    public required string ModId { get; set; }
    public required string Slug { get; set; }

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

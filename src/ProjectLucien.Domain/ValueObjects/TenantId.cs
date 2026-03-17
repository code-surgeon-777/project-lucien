namespace ProjectLucien.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a Tenant aggregate root.
/// Immutable value object that wraps a Guid.
/// </summary>
public sealed class TenantId : IEquatable<TenantId>
{
    public Guid Value { get; }

    private TenantId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Factory method to create a new TenantId with a new Guid.
    /// </summary>
    public static TenantId New() => new(Guid.NewGuid());

    /// <summary>
    /// Factory method to create a TenantId from an existing Guid.
    /// </summary>
    public static TenantId From(Guid guid) => new(guid);

    /// <summary>
    /// Factory method to create a TenantId from a string representation of a Guid.
    /// </summary>
    public static TenantId From(string guidString) => new(Guid.Parse(guidString));

    public bool Equals(TenantId? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj) => obj is TenantId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public static bool operator ==(TenantId? left, TenantId? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(TenantId? left, TenantId? right) => !(left == right);

    public static implicit operator Guid(TenantId tenantId) => tenantId.Value;
}

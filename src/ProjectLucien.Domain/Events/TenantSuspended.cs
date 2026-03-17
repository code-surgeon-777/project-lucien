using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Events;

/// <summary>
/// Event emitted when a tenant is suspended.
/// </summary>
public sealed record TenantSuspended : IDomainEvent
{
    /// <summary>
    /// The unique identifier of the suspended tenant.
    /// </summary>
    public TenantId TenantId { get; }

    /// <summary>
    /// The reason why the tenant was suspended.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// The timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    public TenantSuspended(TenantId tenantId, string reason)
    {
        TenantId = tenantId;
        Reason = reason;
        OccurredAt = DateTime.UtcNow;
    }
}

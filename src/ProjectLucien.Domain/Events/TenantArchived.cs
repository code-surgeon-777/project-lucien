using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Events;

/// <summary>
/// Event emitted when a tenant is archived.
/// </summary>
public sealed record TenantArchived : IDomainEvent
{
    /// <summary>
    /// The unique identifier of the archived tenant.
    /// </summary>
    public TenantId TenantId { get; }

    /// <summary>
    /// The reason why the tenant was archived.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// The timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    public TenantArchived(TenantId tenantId, string reason)
    {
        TenantId = tenantId;
        Reason = reason;
        OccurredAt = DateTime.UtcNow;
    }
}

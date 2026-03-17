using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Events;

/// <summary>
/// Event emitted when a tenant is activated.
/// </summary>
public sealed record TenantActivated : IDomainEvent
{
    /// <summary>
    /// The unique identifier of the activated tenant.
    /// </summary>
    public TenantId TenantId { get; }

    /// <summary>
    /// The timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    public TenantActivated(TenantId tenantId)
    {
        TenantId = tenantId;
        OccurredAt = DateTime.UtcNow;
    }
}

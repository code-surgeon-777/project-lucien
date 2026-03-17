using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Events;

/// <summary>
/// Event emitted when a new tenant is created.
/// </summary>
public sealed record TenantCreated : IDomainEvent
{
    /// <summary>
    /// The unique identifier of the created tenant.
    /// </summary>
    public TenantId TenantId { get; }

    /// <summary>
    /// The name of the tenant.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The subscription plan for the tenant.
    /// </summary>
    public TenantPlan Plan { get; }

    /// <summary>
    /// The timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    public TenantCreated(TenantId tenantId, string name, TenantPlan plan)
    {
        TenantId = tenantId;
        Name = name;
        Plan = plan;
        OccurredAt = DateTime.UtcNow;
    }
}

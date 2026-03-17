using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Events;

/// <summary>
/// Event emitted when an API key is added to a tenant.
/// </summary>
public sealed record TenantApiKeyAdded : IDomainEvent
{
    /// <summary>
    /// The unique identifier of the tenant.
    /// </summary>
    public TenantId TenantId { get; }

    /// <summary>
    /// The API key that was added.
    /// </summary>
    public ApiKey ApiKey { get; }

    /// <summary>
    /// The timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    public TenantApiKeyAdded(TenantId tenantId, ApiKey apiKey)
    {
        TenantId = tenantId;
        ApiKey = apiKey;
        OccurredAt = DateTime.UtcNow;
    }
}

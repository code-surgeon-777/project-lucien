using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Events;

/// <summary>
/// Event emitted when an API key is revoked from a tenant.
/// </summary>
public sealed record TenantApiKeyRevoked : IDomainEvent
{
    /// <summary>
    /// The unique identifier of the tenant.
    /// </summary>
    public TenantId TenantId { get; }

    /// <summary>
    /// The API key that was revoked.
    /// </summary>
    public ApiKey ApiKey { get; }

    /// <summary>
    /// The reason why the API key was revoked.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// The timestamp when the event occurred.
    /// </summary>
    public DateTime OccurredAt { get; }

    public TenantApiKeyRevoked(TenantId tenantId, ApiKey apiKey, string reason)
    {
        TenantId = tenantId;
        ApiKey = apiKey;
        Reason = reason;
        OccurredAt = DateTime.UtcNow;
    }
}

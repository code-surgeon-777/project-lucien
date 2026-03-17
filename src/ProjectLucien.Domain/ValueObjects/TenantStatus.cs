namespace ProjectLucien.Domain.ValueObjects;

/// <summary>
/// Represents the lifecycle status of a tenant.
/// </summary>
public enum TenantStatus
{
    /// <summary>
    /// Tenant has been created but not yet activated.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Tenant is active and fully operational.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Tenant has been suspended (e.g., due to non-payment or policy violation).
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Tenant has been archived and is no longer active.
    /// </summary>
    Archived = 3
}

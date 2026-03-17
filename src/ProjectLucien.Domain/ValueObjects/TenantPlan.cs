namespace ProjectLucien.Domain.ValueObjects;

/// <summary>
/// Represents the subscription plan tier for a tenant.
/// </summary>
public enum TenantPlan
{
    /// <summary>
    /// Free tier with basic features.
    /// </summary>
    Free = 0,

    /// <summary>
    /// Pro tier with additional features and higher limits.
    /// </summary>
    Pro = 1,

    /// <summary>
    /// Enterprise tier with full features and custom support.
    /// </summary>
    Enterprise = 2
}

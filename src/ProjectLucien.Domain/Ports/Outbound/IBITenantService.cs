using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Outbound;

/// <summary>
/// Outbound port for business intelligence tenant service operations.
/// </summary>
public interface IBITenantService
{
    /// <summary>
    /// Records tenant creation in the BI system.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <param name="plan">The subscription plan for the tenant.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RecordTenantCreatedAsync(TenantId tenantId, TenantPlan plan);

    /// <summary>
    /// Records tenant activation in the BI system.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RecordTenantActivatedAsync(TenantId tenantId);

    /// <summary>
    /// Records tenant suspension in the BI system.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <param name="reason">The reason for suspension.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RecordTenantSuspendedAsync(TenantId tenantId, string reason);

    /// <summary>
    /// Records tenant archival in the BI system.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RecordTenantArchivedAsync(TenantId tenantId);
}

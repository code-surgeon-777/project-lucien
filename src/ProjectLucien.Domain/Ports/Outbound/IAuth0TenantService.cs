using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Outbound;

/// <summary>
/// Outbound port for integrating with Auth0 tenant management.
/// </summary>
public interface IAuth0TenantService
{
    /// <summary>
    /// Creates a new tenant in Auth0.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <param name="name">The name of the tenant.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateTenantAsync(TenantId tenantId, string name);

    /// <summary>
    /// Activates a tenant in Auth0.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ActivateTenantAsync(TenantId tenantId);

    /// <summary>
    /// Suspends a tenant in Auth0.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SuspendTenantAsync(TenantId tenantId);

    /// <summary>
    /// Archives a tenant in Auth0.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ArchiveTenantAsync(TenantId tenantId);
}

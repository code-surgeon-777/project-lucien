using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Inbound;

/// <summary>
/// Inbound port for activating a tenant.
/// </summary>
public interface IActivateTenantUseCase
{
    /// <summary>
    /// Activates a tenant, making it fully operational.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant to activate.</param>
    Task ExecuteAsync(TenantId tenantId);
}

using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Inbound;

/// <summary>
/// Inbound port for suspending a tenant.
/// </summary>
public interface ISuspendTenantUseCase
{
    /// <summary>
    /// Suspends a tenant, typically due to non-payment or policy violation.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant to suspend.</param>
    /// <param name="reason">The reason for suspension.</param>
    Task ExecuteAsync(TenantId tenantId, string reason);
}

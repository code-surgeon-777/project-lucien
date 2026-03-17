using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Inbound;

/// <summary>
/// Inbound port for archiving a tenant.
/// </summary>
public interface IArchiveTenantUseCase
{
    /// <summary>
    /// Archives a tenant, marking it as no longer active.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant to archive.</param>
    Task ExecuteAsync(TenantId tenantId);
}

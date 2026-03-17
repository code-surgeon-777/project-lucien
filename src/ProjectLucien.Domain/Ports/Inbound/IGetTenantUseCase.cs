using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Inbound;

/// <summary>
/// Inbound port for retrieving a tenant by ID.
/// </summary>
public interface IGetTenantUseCase
{
    /// <summary>
    /// Retrieves a tenant by its unique identifier.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>The tenant if found, otherwise null.</returns>
    Task<Tenant?> ExecuteAsync(TenantId tenantId);
}

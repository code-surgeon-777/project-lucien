using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Inbound;

/// <summary>
/// Inbound port for creating a new tenant.
/// </summary>
public interface ICreateTenantUseCase
{
    /// <summary>
    /// Creates a new tenant with the specified name, slug, plan, and region.
    /// </summary>
    /// <param name="name">The name of the tenant.</param>
    /// <param name="slug">The URL-safe slug for the tenant.</param>
    /// <param name="plan">The subscription plan for the tenant.</param>
    /// <param name="region">The geographical region for the tenant.</param>
    /// <returns>The created tenant entity.</returns>
    Task<Tenant> ExecuteAsync(string name, string slug, TenantPlan plan, string region);
}

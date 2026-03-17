using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Outbound;

/// <summary>
/// Outbound port for persisting and retrieving tenant aggregates.
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Retrieves a tenant by its unique identifier.
    /// </summary>
    Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a tenant by its slug.
    /// </summary>
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default);

    /// <summary>
    /// Retrieves a tenant by its API key.
    /// </summary>
    Task<Tenant?> GetByApiKeyAsync(string hashedApiKey, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all tenants.
    /// </summary>
    Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Adds a new tenant to the persistence store.
    /// </summary>
    Task AddAsync(Tenant tenant, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing tenant in the persistence store.
    /// </summary>
    Task UpdateAsync(Tenant tenant, CancellationToken ct = default);

    /// <summary>
    /// Deletes a tenant from the persistence store.
    /// </summary>
    Task DeleteAsync(TenantId id, CancellationToken ct = default);

    /// <summary>
    /// Checks if a tenant with the given slug exists.
    /// </summary>
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);
}

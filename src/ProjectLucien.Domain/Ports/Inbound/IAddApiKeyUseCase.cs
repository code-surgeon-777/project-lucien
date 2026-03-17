using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Ports.Inbound;

/// <summary>
/// Inbound port for adding an API key to a tenant.
/// </summary>
public interface IAddApiKeyUseCase
{
    /// <summary>
    /// Adds an API key to the specified tenant.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <param name="keyName">The name of the API key.</param>
    /// <param name="expiresAt">Optional expiration date for the key.</param>
    Task ExecuteAsync(TenantId tenantId, string keyName, DateTime? expiresAt = null);
}

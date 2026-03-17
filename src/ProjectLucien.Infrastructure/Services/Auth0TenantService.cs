using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;
using ProjectLucien.Infrastructure.Auth0;

namespace ProjectLucien.Infrastructure.Services;

/// <summary>
/// Stub implementation of Auth0 tenant service for tenant management integration.
/// </summary>
public class Auth0TenantService : IAuth0TenantService
{
    private readonly Auth0Config _config;

    public Auth0TenantService(Auth0Config config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <inheritdoc />
    public Task CreateTenantAsync(TenantId tenantId, string name)
    {
        // Stub implementation - to be extended with actual Auth0 API integration
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ActivateTenantAsync(TenantId tenantId)
    {
        // Stub implementation - to be extended with actual Auth0 API integration
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task SuspendTenantAsync(TenantId tenantId)
    {
        // Stub implementation - to be extended with actual Auth0 API integration
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ArchiveTenantAsync(TenantId tenantId)
    {
        // Stub implementation - to be extended with actual Auth0 API integration
        return Task.CompletedTask;
    }
}

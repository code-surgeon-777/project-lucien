using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.UseCases;

/// <summary>
/// Use case for adding an API key to a tenant.
/// </summary>
public sealed class AddApiKeyUseCase : IAddApiKeyUseCase
{
    private readonly ITenantRepository _tenantRepository;

    public AddApiKeyUseCase(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task ExecuteAsync(TenantId tenantId, string keyName, DateTime? expiresAt = null)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant is null)
        {
            throw new InvalidOperationException("Tenant not found.");
        }

        // Generate a key hash (in a real implementation, this would be from a secure key generation service)
        var keyHash = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        // Create the API key using the domain factory
        var apiKey = ApiKey.Create(keyName, keyHash, expiresAt);

        // Add the API key to the tenant (this also emits the domain event)
        tenant.AddApiKey(apiKey);

        // Persist via repository
        await _tenantRepository.UpdateAsync(tenant);
    }
}

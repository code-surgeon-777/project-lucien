using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.UseCases;

/// <summary>
/// Use case for activating a tenant.
/// </summary>
public sealed class ActivateTenantUseCase : IActivateTenantUseCase
{
    private readonly ITenantRepository _tenantRepository;

    public ActivateTenantUseCase(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task ExecuteAsync(TenantId tenantId)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant is null)
        {
            throw new InvalidOperationException("Tenant not found.");
        }

        tenant.Activate();
        await _tenantRepository.UpdateAsync(tenant);
    }
}

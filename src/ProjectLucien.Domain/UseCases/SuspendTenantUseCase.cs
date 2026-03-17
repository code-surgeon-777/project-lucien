using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.UseCases;

/// <summary>
/// Use case for suspending a tenant.
/// </summary>
public sealed class SuspendTenantUseCase : ISuspendTenantUseCase
{
    private readonly ITenantRepository _tenantRepository;

    public SuspendTenantUseCase(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task ExecuteAsync(TenantId tenantId, string reason)
    {
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);
        if (tenant is null)
        {
            throw new InvalidOperationException("Tenant not found.");
        }

        tenant.Suspend(reason);
        await _tenantRepository.UpdateAsync(tenant);
    }
}

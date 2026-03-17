using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.UseCases;

/// <summary>
/// Use case for archiving a tenant.
/// </summary>
public sealed class ArchiveTenantUseCase : IArchiveTenantUseCase
{
    private readonly ITenantRepository _tenantRepository;

    public ArchiveTenantUseCase(ITenantRepository tenantRepository)
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

        tenant.Archive();
        await _tenantRepository.UpdateAsync(tenant);
    }
}

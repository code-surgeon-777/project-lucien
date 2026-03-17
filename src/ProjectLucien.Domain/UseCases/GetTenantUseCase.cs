using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.UseCases;

/// <summary>
/// Use case for retrieving a tenant by ID.
/// </summary>
public sealed class GetTenantUseCase : IGetTenantUseCase
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantUseCase(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Tenant?> ExecuteAsync(TenantId tenantId)
    {
        return await _tenantRepository.GetByIdAsync(tenantId);
    }
}

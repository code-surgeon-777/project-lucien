using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.UseCases;

/// <summary>
/// Use case for creating a new tenant.
/// </summary>
public sealed class CreateTenantUseCase : ICreateTenantUseCase
{
    private readonly ITenantRepository _tenantRepository;

    public CreateTenantUseCase(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Tenant> ExecuteAsync(string name, string slug, TenantPlan plan, string region)
    {
        // Check if slug already exists
        if (await _tenantRepository.ExistsBySlugAsync(slug))
        {
            throw new InvalidOperationException($"Slug '{slug}' already exists.");
        }

        // Create the tenant using the domain factory
        var tenant = Tenant.Create(name, slug, plan, region);

        // Persist via repository
        await _tenantRepository.AddAsync(tenant);

        return tenant;
    }
}

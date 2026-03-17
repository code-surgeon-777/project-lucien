using Microsoft.EntityFrameworkCore;
using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;
using ProjectLucien.Infrastructure;
using ProjectLucien.Infrastructure.Mappers;

namespace ProjectLucien.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of ITenantRepository.
/// </summary>
public class TenantRepository : ITenantRepository
{
    private readonly ProjectLucienDbContext _dbContext;

    public TenantRepository(ProjectLucienDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tenant?> GetByIdAsync(TenantId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Tenants
            .Include(t => t.ApiKeys)
            .Include(t => t.FeatureFlags)
            .FirstOrDefaultAsync(t => t.Id == id.Value, ct);

        return entity is null ? null : TenantMapper.ToDomain(entity);
    }

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var entity = await _dbContext.Tenants
            .Include(t => t.ApiKeys)
            .Include(t => t.FeatureFlags)
            .FirstOrDefaultAsync(t => t.Slug == slug, ct);

        return entity is null ? null : TenantMapper.ToDomain(entity);
    }

    public async Task<Tenant?> GetByApiKeyAsync(string hashedApiKey, CancellationToken ct = default)
    {
        var entity = await _dbContext.Tenants
            .Include(t => t.ApiKeys)
            .Include(t => t.FeatureFlags)
            .FirstOrDefaultAsync(t => t.ApiKeys.Any(k => k.KeyHash == hashedApiKey && !k.IsRevoked), ct);

        return entity is null ? null : TenantMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _dbContext.Tenants
            .Include(t => t.ApiKeys)
            .Include(t => t.FeatureFlags)
            .ToListAsync(ct);

        return entities.Select(TenantMapper.ToDomain).ToList();
    }

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default)
    {
        var entity = TenantMapper.ToEntity(tenant);
        await _dbContext.Tenants.AddAsync(entity, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        var entity = TenantMapper.ToEntity(tenant);

        // Get existing entity to update
        var existingEntity = await _dbContext.Tenants
            .Include(t => t.ApiKeys)
            .Include(t => t.FeatureFlags)
            .FirstOrDefaultAsync(t => t.Id == entity.Id, ct);

        if (existingEntity is null)
        {
            throw new InvalidOperationException($"Tenant with ID {tenant.Id.Value} not found.");
        }

        // Update main properties
        existingEntity.Name = entity.Name;
        existingEntity.Slug = entity.Slug;
        existingEntity.Status = entity.Status;
        existingEntity.Plan = entity.Plan;
        existingEntity.Region = entity.Region;
        existingEntity.Auth0OrganizationId = entity.Auth0OrganizationId;
        existingEntity.UpdatedAt = entity.UpdatedAt;

        // Clear and update API keys
        _dbContext.ApiKeys.RemoveRange(existingEntity.ApiKeys);
        foreach (var apiKey in entity.ApiKeys)
        {
            _dbContext.ApiKeys.Add(apiKey);
        }

        // Clear and update feature flags
        _dbContext.TenantFeatureFlags.RemoveRange(existingEntity.FeatureFlags);
        foreach (var featureFlag in entity.FeatureFlags)
        {
            _dbContext.TenantFeatureFlags.Add(featureFlag);
        }

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TenantId id, CancellationToken ct = default)
    {
        var entity = await _dbContext.Tenants.FindAsync(new object[] { id.Value }, ct);
        if (entity is not null)
        {
            _dbContext.Tenants.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _dbContext.Tenants.AnyAsync(t => t.Slug == slug, ct);
    }
}

using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.ValueObjects;
using ProjectLucien.Infrastructure.Entities;

namespace ProjectLucien.Infrastructure.Mappers;

/// <summary>
/// Mapper for converting between domain Tenant and persistence TenantEntity.
/// </summary>
public static class TenantMapper
{
    /// <summary>
    /// Maps a TenantEntity to a domain Tenant.
    /// </summary>
    public static Tenant ToDomain(TenantEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var tenant = new Tenant(
            TenantId.From(entity.Id),
            entity.Name,
            entity.Slug,
            (TenantStatus)entity.Status,
            (TenantPlan)entity.Plan,
            entity.Region,
            entity.Auth0OrganizationId,
            entity.CreatedAt,
            entity.UpdatedAt
        );

        // Map API keys
        foreach (var apiKeyEntity in entity.ApiKeys)
        {
            var apiKey = ApiKey.FromExisting(
                apiKeyEntity.Id,
                apiKeyEntity.Name,
                apiKeyEntity.KeyHash,
                apiKeyEntity.CreatedAt,
                apiKeyEntity.ExpiresAt,
                apiKeyEntity.IsRevoked
            );
            tenant.AddApiKeyInternal(apiKey);
        }

        // Map feature flags
        foreach (var featureFlagEntity in entity.FeatureFlags)
        {
            tenant.AddFeatureFlagInternal(featureFlagEntity.FeatureName);
        }

        return tenant;
    }

    /// <summary>
    /// Maps a domain Tenant to a TenantEntity.
    /// </summary>
    public static TenantEntity ToEntity(Tenant tenant)
    {
        if (tenant == null)
            throw new ArgumentNullException(nameof(tenant));

        var entity = new TenantEntity
        {
            Id = tenant.Id.Value,
            Name = tenant.Name,
            Slug = tenant.Slug,
            Status = (int)tenant.Status,
            Plan = (int)tenant.Plan,
            Region = tenant.Region,
            Auth0OrganizationId = tenant.Auth0OrganizationId,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt
        };

        // Map API keys
        foreach (var apiKey in tenant.ApiKeys)
        {
            entity.ApiKeys.Add(new ApiKeyEntity
            {
                Id = apiKey.Id,
                TenantId = tenant.Id.Value,
                Name = apiKey.Name,
                KeyHash = apiKey.KeyHash,
                CreatedAt = apiKey.CreatedAt,
                ExpiresAt = apiKey.ExpiresAt,
                IsRevoked = apiKey.IsRevoked
            });
        }

        // Map feature flags
        foreach (var featureName in tenant.FeatureFlags)
        {
            entity.FeatureFlags.Add(new TenantFeatureFlagEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id.Value,
                FeatureName = featureName,
                CreatedAt = DateTime.UtcNow
            });
        }

        return entity;
    }
}

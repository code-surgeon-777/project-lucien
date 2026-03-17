using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLucien.Infrastructure.Entities;

/// <summary>
/// EF Core entity for persisting tenant data.
/// </summary>
[Table("Tenants")]
public class TenantEntity
{
    /// <summary>
    /// The unique identifier of the tenant.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the tenant.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-safe unique identifier for the tenant.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// The current lifecycle status of the tenant.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// The subscription plan tier for the tenant.
    /// </summary>
    public int Plan { get; set; }

    /// <summary>
    /// The geographical region for the tenant's resources.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Linked Auth0 organization ID.
    /// </summary>
    [MaxLength(255)]
    public string? Auth0OrganizationId { get; set; }

    /// <summary>
    /// The timestamp when the tenant was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The timestamp when the tenant was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Collection of API keys associated with this tenant.
    /// </summary>
    public virtual ICollection<ApiKeyEntity> ApiKeys { get; set; } = new List<ApiKeyEntity>();

    /// <summary>
    /// Collection of feature flags enabled for this tenant.
    /// </summary>
    public virtual ICollection<TenantFeatureFlagEntity> FeatureFlags { get; set; } = new List<TenantFeatureFlagEntity>();
}

/// <summary>
/// EF Core entity for persisting API key data.
/// </summary>
[Table("ApiKeys")]
public class ApiKeyEntity
{
    /// <summary>
    /// The unique identifier of the API key.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the tenant this API key belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The name of the API key.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The hashed value of the API key.
    /// </summary>
    [Required]
    [MaxLength(512)]
    public string KeyHash { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the API key was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The expiration date of the API key (optional).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Whether the API key has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Navigation property to the tenant.
    /// </summary>
    [ForeignKey(nameof(TenantId))]
    public virtual TenantEntity? Tenant { get; set; }
}

/// <summary>
/// EF Core entity for persisting feature flag data.
/// </summary>
[Table("TenantFeatureFlags")]
public class TenantFeatureFlagEntity
{
    /// <summary>
    /// The unique identifier of the feature flag.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the tenant this feature flag belongs to.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// The name of the feature flag.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string FeatureName { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the feature flag was enabled.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property to the tenant.
    /// </summary>
    [ForeignKey(nameof(TenantId))]
    public virtual TenantEntity? Tenant { get; set; }
}

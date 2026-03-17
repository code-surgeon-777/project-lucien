using ProjectLucien.Domain.Events;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Domain.Entities;

/// <summary>
/// Tenant aggregate root - the central entity in Project Lucien.
/// All changes to tenant state must go through this aggregate.
/// </summary>
public class Tenant
{
    private readonly List<IDomainEvent> _events = new();
    private readonly List<ApiKey> _apiKeys = new();
    private readonly List<string> _featureFlags = new();

    /// <summary>
    /// Private constructor for domain-driven creation. Use factory methods instead.
    /// </summary>
    private Tenant() { }

    /// <summary>
    /// Public constructor for reconstruction from persistence.
    /// </summary>
    public Tenant(
        TenantId id,
        string name,
        string slug,
        TenantStatus status,
        TenantPlan plan,
        string region,
        string? auth0OrganizationId,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Status = status;
        Plan = plan;
        Region = region;
        Auth0OrganizationId = auth0OrganizationId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// The unique identifier of the tenant.
    /// </summary>
    public TenantId Id { get; private set; } = null!;

    /// <summary>
    /// The display name of the tenant.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// URL-safe unique identifier for the tenant.
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// The current lifecycle status of the tenant.
    /// </summary>
    public TenantStatus Status { get; private set; }

    /// <summary>
    /// The subscription plan tier for the tenant.
    /// </summary>
    public TenantPlan Plan { get; private set; }

    /// <summary>
    /// The geographical region for the tenant's resources.
    /// </summary>
    public string Region { get; private set; } = string.Empty;

    /// <summary>
    /// Linked Auth0 organization ID.
    /// </summary>
    public string? Auth0OrganizationId { get; private set; }

    /// <summary>
    /// Linked API keys for the tenant.
    /// </summary>
    public IReadOnlyList<ApiKey> ApiKeys => _apiKeys.AsReadOnly();

    /// <summary>
    /// Enabled feature flags for the tenant.
    /// </summary>
    public IReadOnlyList<string> FeatureFlags => _featureFlags.AsReadOnly();

    /// <summary>
    /// The timestamp when the tenant was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// The timestamp when the tenant was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Read-only collection of domain events emitted by this tenant.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();

    /// <summary>
    /// Factory method to create a new tenant.
    /// </summary>
    public static Tenant Create(string name, string slug, TenantPlan plan, string region)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Tenant slug cannot be empty", nameof(slug));

        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Tenant region cannot be empty", nameof(region));

        var tenant = new Tenant
        {
            Id = TenantId.New(),
            Name = name.Trim(),
            Slug = slug.Trim().ToLowerInvariant(),
            Plan = plan,
            Region = region.Trim(),
            Status = TenantStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        tenant._events.Add(new TenantCreated(tenant.Id, tenant.Name, tenant.Plan));
        return tenant;
    }

    /// <summary>
    /// Activates the tenant, allowing it to become operational.
    /// </summary>
    public void Activate()
    {
        if (Status != TenantStatus.Pending && Status != TenantStatus.Suspended)
            throw new InvalidOperationException($"Cannot activate tenant from {Status} status.");

        Status = TenantStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new TenantActivated(Id));
    }

    /// <summary>
    /// Suspends the tenant, typically due to non-payment or policy violation.
    /// </summary>
    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Suspension reason cannot be empty", nameof(reason));

        if (Status != TenantStatus.Active)
            throw new InvalidOperationException($"Only active tenants can be suspended. Current status: {Status}.");

        Status = TenantStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new TenantSuspended(Id, reason));
    }

    /// <summary>
    /// Archives the tenant, marking it as no longer active.
    /// </summary>
    public void Archive()
    {
        if (Status == TenantStatus.Archived)
            throw new InvalidOperationException("Tenant is already archived.");

        Status = TenantStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new TenantArchived(Id, "Tenant archived"));
    }

    /// <summary>
    /// Adds an API key to the tenant.
    /// </summary>
    public void AddApiKey(ApiKey apiKey)
    {
        if (apiKey == null)
            throw new ArgumentNullException(nameof(apiKey));

        _apiKeys.Add(apiKey);
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new TenantApiKeyAdded(Id, apiKey));
    }

    /// <summary>
    /// Revokes an API key from the tenant.
    /// </summary>
    public void RevokeApiKey(Guid apiKeyId, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Revocation reason cannot be empty", nameof(reason));

        var apiKey = _apiKeys.FirstOrDefault(k => k.Id == apiKeyId);
        if (apiKey == null)
            throw new InvalidOperationException("API key not found.");

        var revokedKey = apiKey.Revoke();
        _apiKeys.Remove(apiKey);
        _apiKeys.Add(revokedKey);
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new TenantApiKeyRevoked(Id, revokedKey, reason));
    }

    /// <summary>
    /// Links an Auth0 organization to this tenant.
    /// </summary>
    public void LinkAuth0Organization(string auth0OrganizationId)
    {
        if (string.IsNullOrWhiteSpace(auth0OrganizationId))
            throw new ArgumentException("Auth0 organization ID cannot be empty", nameof(auth0OrganizationId));

        Auth0OrganizationId = auth0OrganizationId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Enables a feature flag for the tenant.
    /// </summary>
    public void EnableFeature(string featureName)
    {
        if (string.IsNullOrWhiteSpace(featureName))
            throw new ArgumentException("Feature name cannot be empty", nameof(featureName));

        if (!_featureFlags.Contains(featureName))
        {
            _featureFlags.Add(featureName);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Internal method to add an API key (used by mapper).
    /// </summary>
    public void AddApiKeyInternal(ApiKey apiKey)
    {
        _apiKeys.Add(apiKey);
    }

    /// <summary>
    /// Internal method to add a feature flag (used by mapper).
    /// </summary>
    public void AddFeatureFlagInternal(string featureName)
    {
        _featureFlags.Add(featureName);
    }

    /// <summary>
    /// Clears all domain events after they have been processed.
    /// </summary>
    public void ClearDomainEvents()
    {
        _events.Clear();
    }
}

using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Infrastructure.Services;

/// <summary>
/// Stub implementation of BI tenant service for business intelligence tracking.
/// </summary>
public class BITenantService : IBITenantService
{
    /// <inheritdoc />
    public Task RecordTenantCreatedAsync(TenantId tenantId, TenantPlan plan)
    {
        // Stub implementation - to be extended with actual BI system integration
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RecordTenantActivatedAsync(TenantId tenantId)
    {
        // Stub implementation - to be extended with actual BI system integration
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RecordTenantSuspendedAsync(TenantId tenantId, string reason)
    {
        // Stub implementation - to be extended with actual BI system integration
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RecordTenantArchivedAsync(TenantId tenantId)
    {
        // Stub implementation - to be extended with actual BI system integration
        return Task.CompletedTask;
    }
}

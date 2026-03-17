using Microsoft.EntityFrameworkCore;
using ProjectLucien.Infrastructure.Entities;

namespace ProjectLucien.Infrastructure;

/// <summary>
/// EF Core DbContext for Project Lucien.
/// </summary>
public class ProjectLucienDbContext : DbContext
{
    public ProjectLucienDbContext(DbContextOptions<ProjectLucienDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tenants table.
    /// </summary>
    public DbSet<TenantEntity> Tenants => Set<TenantEntity>();

    /// <summary>
    /// API Keys table.
    /// </summary>
    public DbSet<ApiKeyEntity> ApiKeys => Set<ApiKeyEntity>();

    /// <summary>
    /// Tenant Feature Flags table.
    /// </summary>
    public DbSet<TenantFeatureFlagEntity> TenantFeatureFlags => Set<TenantFeatureFlagEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure TenantEntity
        modelBuilder.Entity<TenantEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Region).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Auth0OrganizationId).HasMaxLength(255);
            entity.HasIndex(e => e.Slug).IsUnique();

            // Configure relationships
            entity.HasMany(e => e.ApiKeys)
                .WithOne(a => a.Tenant)
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.FeatureFlags)
                .WithOne(f => f.Tenant)
                .HasForeignKey(f => f.TenantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ApiKeyEntity
        modelBuilder.Entity<ApiKeyEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.KeyHash).IsRequired().HasMaxLength(512);
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
        });

        // Configure TenantFeatureFlagEntity
        modelBuilder.Entity<TenantFeatureFlagEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeatureName).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => new { e.TenantId, e.FeatureName }).IsUnique();
        });
    }
}

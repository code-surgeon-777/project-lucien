using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProjectLucien.Domain.ValueObjects;
using ProjectLucien.Infrastructure;

namespace ProjectLucien.Api.Middleware;

/// <summary>
/// Middleware for validating API keys on incoming HTTP requests.
/// Checks for valid API key in request headers and validates against tenant's active API keys.
/// </summary>
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;

    // Header name for API key
    private const string ApiKeyHeader = "X-Api-Key";

    // Paths that don't require API key validation
    private static readonly HashSet<string> ExcludedPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/health",
        "/health/live",
        "/health/ready",
        "/swagger",
        "/swagger/",
        "/swagger/index.html",
        "/swagger.json",
        "/odata",
        "/"
    };

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to validate API keys.
    /// </summary>
    public async Task InvokeAsync(HttpContext context, ProjectLucienDbContext dbContext)
    {
        // Skip validation for excluded paths
        if (ShouldSkipValidation(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Get API key from header
        var apiKey = context.Request.Headers[ApiKeyHeader].FirstOrDefault();

        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Missing API key in request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API key is required" });
            return;
        }

        // Validate API key against database
        var isValid = await ValidateApiKeyAsync(apiKey, dbContext, context);

        if (!isValid)
        {
            _logger.LogWarning("Invalid API key attempt to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid or expired API key" });
            return;
        }

        _logger.LogDebug("Valid API key authenticated for request to {Path}", context.Request.Path);
        await _next(context);
    }

    /// <summary>
    /// Determines if the request path should skip API key validation.
    /// </summary>
    private static bool ShouldSkipValidation(PathString path)
    {
        if (path == null)
            return true;

        var pathValue = path.Value;
        if (string.IsNullOrEmpty(pathValue))
            return true;

        // Check exact matches
        if (ExcludedPaths.Contains(pathValue))
            return true;

        // Check if path starts with any excluded prefix
        foreach (var excluded in ExcludedPaths)
        {
            if (pathValue.StartsWith(excluded, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Validates the API key against the database.
    /// Checks if the key exists, is active, not expired, and not revoked.
    /// </summary>
    private async Task<bool> ValidateApiKeyAsync(string apiKey, ProjectLucienDbContext dbContext, HttpContext httpContext)
    {
        try
        {
            // Hash the incoming API key for comparison
            var keyHash = HashApiKey(apiKey);

            // Find tenant with matching API key hash
            var tenant = await dbContext.Tenants
                .Include(t => t.ApiKeys)
                .FirstOrDefaultAsync(t => t.ApiKeys.Any(k => k.KeyHash == keyHash));

            if (tenant == null)
            {
                _logger.LogWarning("API key not found in any tenant");
                return false;
            }

            // Check tenant status
            if (tenant.Status != (int)TenantStatus.Active)
            {
                _logger.LogWarning("Tenant {TenantId} is not active (status: {Status})",
                    tenant.Id, tenant.Status);
                return false;
            }

            // Find the specific API key
            var apiKeyEntity = tenant.ApiKeys.FirstOrDefault(k => k.KeyHash == keyHash);

            if (apiKeyEntity == null)
            {
                _logger.LogWarning("API key not found in tenant {TenantId}", tenant.Id);
                return false;
            }

            // Check if key is revoked
            if (apiKeyEntity.IsRevoked)
            {
                _logger.LogWarning("API key is revoked for tenant {TenantId}", tenant.Id);
                return false;
            }

            // Check if key is expired
            if (apiKeyEntity.ExpiresAt.HasValue && apiKeyEntity.ExpiresAt.Value < DateTime.UtcNow)
            {
                _logger.LogWarning("API key is expired for tenant {TenantId}", tenant.Id);
                return false;
            }

            // Store tenant ID in context for downstream use
            httpContext.Items["TenantId"] = tenant.Id;
            httpContext.Items["TenantName"] = tenant.Name;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating API key");
            return false;
        }
    }

    /// <summary>
    /// Hashes an API key using SHA256.
    /// </summary>
    private static string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(apiKey);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}

/// <summary>
/// Extension methods for registering API key middleware.
/// </summary>
public static class ApiKeyMiddlewareExtensions
{
    /// <summary>
    /// Adds API key middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyMiddleware>();
    }
}

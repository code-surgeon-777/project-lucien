using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.ValueObjects;

namespace ProjectLucien.Api.Controllers;

/// <summary>
/// OData controller for tenant operations.
/// Supports full OData query syntax: $filter, $select, $orderby, $top, $skip, $count.
/// </summary>
public class TenantsController : ODataController
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ICreateTenantUseCase _createTenantUseCase;
    private readonly IGetTenantUseCase _getTenantUseCase;
    private readonly IActivateTenantUseCase _activateTenantUseCase;
    private readonly ISuspendTenantUseCase _suspendTenantUseCase;
    private readonly IArchiveTenantUseCase _archiveTenantUseCase;
    private readonly IAddApiKeyUseCase _addApiKeyUseCase;

    public TenantsController(
        ITenantRepository tenantRepository,
        ICreateTenantUseCase createTenantUseCase,
        IGetTenantUseCase getTenantUseCase,
        IActivateTenantUseCase activateTenantUseCase,
        ISuspendTenantUseCase suspendTenantUseCase,
        IArchiveTenantUseCase archiveTenantUseCase,
        IAddApiKeyUseCase addApiKeyUseCase)
    {
        _tenantRepository = tenantRepository;
        _createTenantUseCase = createTenantUseCase;
        _getTenantUseCase = getTenantUseCase;
        _activateTenantUseCase = activateTenantUseCase;
        _suspendTenantUseCase = suspendTenantUseCase;
        _archiveTenantUseCase = archiveTenantUseCase;
        _addApiKeyUseCase = addApiKeyUseCase;
    }

    /// <summary>
    /// Gets all tenants with OData query support.
    /// </summary>
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("odata/Tenants")]
    public async Task<IQueryable<TenantDto>> Get()
    {
        var tenants = await _tenantRepository.GetAllAsync();
        return tenants.Select(t => MapToDto(t)).AsQueryable();
    }

    /// <summary>
    /// Gets a single tenant by ID with OData query support.
    /// </summary>
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("odata/Tenants({key})")]
    public async Task<IActionResult> Get([FromRoute] string key)
    {
        if (!Guid.TryParse(key, out var tenantIdGuid))
        {
            return BadRequest("Invalid tenant ID format");
        }

        var tenantId = TenantId.From(tenantIdGuid);
        var tenant = await _getTenantUseCase.ExecuteAsync(tenantId);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID '{key}' not found");
        }

        return Ok(MapToDto(tenant));
    }

    /// <summary>
    /// Creates a new tenant.
    /// </summary>
    [HttpPost("odata/Tenants")]
    public async Task<IActionResult> Post([FromBody] TenantCreateDto tenantDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!Enum.TryParse<TenantPlan>(tenantDto.Plan, ignoreCase: true, out var plan))
        {
            plan = TenantPlan.Free;
        }

        var tenant = await _createTenantUseCase.ExecuteAsync(
            tenantDto.Name,
            tenantDto.Slug,
            plan,
            tenantDto.Region);

        return Created(MapToDto(tenant));
    }

    /// <summary>
    /// Updates an existing tenant.
    /// </summary>
    [HttpPut("odata/Tenants({key})")]
    public async Task<IActionResult> Put([FromRoute] string key, [FromBody] TenantUpdateDto tenantDto)
    {
        if (!Guid.TryParse(key, out var tenantIdGuid))
        {
            return BadRequest("Invalid tenant ID format");
        }

        var tenantId = TenantId.From(tenantIdGuid);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID '{key}' not found");
        }

        await _tenantRepository.UpdateAsync(tenant);

        return Updated(MapToDto(tenant));
    }

    /// <summary>
    /// Deletes a tenant.
    /// </summary>
    [HttpDelete("odata/Tenants({key})")]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        if (!Guid.TryParse(key, out var tenantIdGuid))
        {
            return BadRequest("Invalid tenant ID format");
        }

        var tenantId = TenantId.From(tenantIdGuid);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID '{key}' not found");
        }

        await _tenantRepository.DeleteAsync(tenantId);

        return NoContent();
    }

    /// <summary>
    /// Activates a tenant.
    /// </summary>
    [HttpPost("odata/Tenants({key})/Activate")]
    public async Task<IActionResult> Activate([FromRoute] string key)
    {
        if (!Guid.TryParse(key, out var tenantIdGuid))
        {
            return BadRequest("Invalid tenant ID format");
        }

        var tenantId = TenantId.From(tenantIdGuid);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID '{key}' not found");
        }

        await _activateTenantUseCase.ExecuteAsync(tenantId);

        return Ok(MapToDto(tenant));
    }

    /// <summary>
    /// Suspends a tenant.
    /// </summary>
    [HttpPost("odata/Tenants({key})/Suspend")]
    public async Task<IActionResult> Suspend([FromRoute] string key, [FromBody] SuspendRequest request)
    {
        if (!Guid.TryParse(key, out var tenantIdGuid))
        {
            return BadRequest("Invalid tenant ID format");
        }

        var tenantId = TenantId.From(tenantIdGuid);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID '{key}' not found");
        }

        await _suspendTenantUseCase.ExecuteAsync(tenantId, request.Reason);

        return Ok(MapToDto(tenant));
    }

    /// <summary>
    /// Archives a tenant.
    /// </summary>
    [HttpPost("odata/Tenants({key})/Archive")]
    public async Task<IActionResult> Archive([FromRoute] string key)
    {
        if (!Guid.TryParse(key, out var tenantIdGuid))
        {
            return BadRequest("Invalid tenant ID format");
        }

        var tenantId = TenantId.From(tenantIdGuid);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID '{key}' not found");
        }

        await _archiveTenantUseCase.ExecuteAsync(tenantId);

        return Ok(MapToDto(tenant));
    }

    /// <summary>
    /// Adds an API key to a tenant.
    /// </summary>
    [HttpPost("odata/Tenants({key})/AddApiKey")]
    public async Task<IActionResult> AddApiKey([FromRoute] string key, [FromBody] AddApiKeyRequest request)
    {
        if (!Guid.TryParse(key, out var tenantIdGuid))
        {
            return BadRequest("Invalid tenant ID format");
        }

        var tenantId = TenantId.From(tenantIdGuid);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound($"Tenant with ID '{key}' not found");
        }

        await _addApiKeyUseCase.ExecuteAsync(tenantId, request.KeyName, request.ExpiresAt);

        return Ok(MapToDto(tenant));
    }

    private static TenantDto MapToDto(Tenant tenant)
    {
        return new TenantDto
        {
            Id = tenant.Id.Value,
            Name = tenant.Name,
            Slug = tenant.Slug,
            Status = tenant.Status.ToString(),
            Plan = tenant.Plan.ToString(),
            Region = tenant.Region,
            Auth0OrganizationId = tenant.Auth0OrganizationId,
            CreatedAt = tenant.CreatedAt,
            UpdatedAt = tenant.UpdatedAt,
            ApiKeys = tenant.ApiKeys.Select(k => new ApiKeyDto
            {
                Id = k.Id,
                Name = k.Name,
                KeyHash = k.KeyHash,
                CreatedAt = k.CreatedAt,
                ExpiresAt = k.ExpiresAt,
                IsRevoked = k.IsRevoked
            }).ToList()
        };
    }
}

/// <summary>
/// Data transfer object for Tenant entity.
/// </summary>
public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? Auth0OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ApiKeyDto> ApiKeys { get; set; } = new();
}

/// <summary>
/// Data transfer object for ApiKey value object.
/// </summary>
public class ApiKeyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}

/// <summary>
/// DTO for creating a new tenant.
/// </summary>
public class TenantCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Plan { get; set; }
    public string Region { get; set; } = string.Empty;
    public string? Auth0OrganizationId { get; set; }
}

/// <summary>
/// DTO for updating a tenant.
/// </summary>
public class TenantUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Plan { get; set; }
    public string Region { get; set; } = string.Empty;
}

/// <summary>
/// DTO for suspending a tenant.
/// </summary>
public class SuspendRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// DTO for adding an API key.
/// </summary>
public class AddApiKeyRequest
{
    public string KeyName { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

using Grpc.Core;
using ProjectLucien.Api.Grpc.Protos;
using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.Ports.Inbound;
using DomainTenantPlan = ProjectLucien.Domain.ValueObjects.TenantPlan;
using DomainTenantStatus = ProjectLucien.Domain.ValueObjects.TenantStatus;
using ProtosTenantPlan = ProjectLucien.Api.Grpc.Protos.TenantPlan;
using ProtosTenantStatus = ProjectLucien.Api.Grpc.Protos.TenantStatus;

namespace ProjectLucien.Api.Grpc.Services;

/// <summary>
/// gRPC service implementation for InternalLibrarian.
/// Maps gRPC requests to domain use cases.
/// </summary>
public class InternalLibrarianService : Protos.InternalLibrarian.InternalLibrarianBase
{
    private readonly ICreateTenantUseCase _createTenantUseCase;
    private readonly IGetTenantUseCase _getTenantUseCase;
    private readonly IActivateTenantUseCase _activateTenantUseCase;
    private readonly ISuspendTenantUseCase _suspendTenantUseCase;
    private readonly IArchiveTenantUseCase _archiveTenantUseCase;
    private readonly IAddApiKeyUseCase _addApiKeyUseCase;
    private readonly ILogger<InternalLibrarianService> _logger;

    public InternalLibrarianService(
        ICreateTenantUseCase createTenantUseCase,
        IGetTenantUseCase getTenantUseCase,
        IActivateTenantUseCase activateTenantUseCase,
        ISuspendTenantUseCase suspendTenantUseCase,
        IArchiveTenantUseCase archiveTenantUseCase,
        IAddApiKeyUseCase addApiKeyUseCase,
        ILogger<InternalLibrarianService> logger)
    {
        _createTenantUseCase = createTenantUseCase;
        _getTenantUseCase = getTenantUseCase;
        _activateTenantUseCase = activateTenantUseCase;
        _suspendTenantUseCase = suspendTenantUseCase;
        _archiveTenantUseCase = archiveTenantUseCase;
        _addApiKeyUseCase = addApiKeyUseCase;
        _logger = logger;
    }

    public override async Task<TenantResponse> CreateTenant(CreateTenantRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating tenant with name: {Name}, slug: {Slug}", request.Name, request.Slug);

        var plan = MapToDomainTenantPlan(request.Plan);
        var tenant = await _createTenantUseCase.ExecuteAsync(request.Name, request.Slug, plan, request.Region);

        return MapToTenantResponse(tenant);
    }

    public override async Task<TenantResponse> GetTenant(GetTenantRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Retrieving tenant with ID: {TenantId}", request.TenantId);

        var tenantId = Domain.ValueObjects.TenantId.From(request.TenantId);
        var tenant = await _getTenantUseCase.ExecuteAsync(tenantId);

        if (tenant == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Tenant with ID {request.TenantId} not found"));
        }

        return MapToTenantResponse(tenant);
    }

    public override async Task<TenantResponse> ActivateTenant(ActivateTenantRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Activating tenant with ID: {TenantId}", request.TenantId);

        var tenantId = Domain.ValueObjects.TenantId.From(request.TenantId);
        await _activateTenantUseCase.ExecuteAsync(tenantId);

        var tenant = await _getTenantUseCase.ExecuteAsync(tenantId);
        return MapToTenantResponse(tenant!);
    }

    public override async Task<TenantResponse> SuspendTenant(SuspendTenantRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Suspending tenant with ID: {TenantId}, reason: {Reason}", request.TenantId, request.Reason);

        var tenantId = Domain.ValueObjects.TenantId.From(request.TenantId);
        await _suspendTenantUseCase.ExecuteAsync(tenantId, request.Reason);

        var tenant = await _getTenantUseCase.ExecuteAsync(tenantId);
        return MapToTenantResponse(tenant!);
    }

    public override async Task<TenantResponse> ArchiveTenant(ArchiveTenantRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Archiving tenant with ID: {TenantId}", request.TenantId);

        var tenantId = Domain.ValueObjects.TenantId.From(request.TenantId);
        await _archiveTenantUseCase.ExecuteAsync(tenantId);

        var tenant = await _getTenantUseCase.ExecuteAsync(tenantId);
        return MapToTenantResponse(tenant!);
    }

    public override async Task<ApiKeyResponse> AddApiKey(AddApiKeyRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Adding API key to tenant with ID: {TenantId}, key name: {KeyName}", request.TenantId, request.KeyName);

        var tenantId = Domain.ValueObjects.TenantId.From(request.TenantId);
        DateTime? expiresAt = null;

        if (request.ExpiresAt > 0)
        {
            expiresAt = DateTimeOffset.FromUnixTimeSeconds(request.ExpiresAt).UtcDateTime;
        }

        await _addApiKeyUseCase.ExecuteAsync(tenantId, request.KeyName, expiresAt);

        var tenant = await _getTenantUseCase.ExecuteAsync(tenantId);
        var apiKey = tenant!.ApiKeys.FirstOrDefault(k => k.Name == request.KeyName);

        if (apiKey == null)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Failed to retrieve the created API key"));
        }

        return MapToApiKeyResponse(apiKey);
    }

    private static TenantResponse MapToTenantResponse(Tenant tenant)
    {
        return new TenantResponse
        {
            TenantId = tenant.Id.Value.ToString(),
            Name = tenant.Name,
            Slug = tenant.Slug,
            Status = MapToProtosTenantStatus(tenant.Status),
            Plan = MapToProtosTenantPlan(tenant.Plan),
            Region = tenant.Region,
            Auth0OrganizationId = tenant.Auth0OrganizationId ?? string.Empty,
            CreatedAt = new DateTimeOffset(tenant.CreatedAt).ToUnixTimeSeconds(),
            UpdatedAt = new DateTimeOffset(tenant.UpdatedAt).ToUnixTimeSeconds(),
            ApiKeys = { tenant.ApiKeys.Select(MapToApiKeyResponse) }
        };
    }

    private static ApiKeyResponse MapToApiKeyResponse(Domain.ValueObjects.ApiKey apiKey)
    {
        return new ApiKeyResponse
        {
            Id = apiKey.Id.ToString(),
            Name = apiKey.Name,
            CreatedAt = new DateTimeOffset(apiKey.CreatedAt).ToUnixTimeSeconds(),
            ExpiresAt = apiKey.ExpiresAt.HasValue
                ? new DateTimeOffset(apiKey.ExpiresAt.Value).ToUnixTimeSeconds()
                : 0,
            IsRevoked = apiKey.IsRevoked
        };
    }

    private static DomainTenantPlan MapToDomainTenantPlan(ProtosTenantPlan plan)
    {
        return plan switch
        {
            ProtosTenantPlan.Free => DomainTenantPlan.Free,
            ProtosTenantPlan.Pro => DomainTenantPlan.Pro,
            ProtosTenantPlan.Enterprise => DomainTenantPlan.Enterprise,
            _ => DomainTenantPlan.Free
        };
    }

    private static ProtosTenantPlan MapToProtosTenantPlan(DomainTenantPlan plan)
    {
        return plan switch
        {
            DomainTenantPlan.Free => ProtosTenantPlan.Free,
            DomainTenantPlan.Pro => ProtosTenantPlan.Pro,
            DomainTenantPlan.Enterprise => ProtosTenantPlan.Enterprise,
            _ => ProtosTenantPlan.Free
        };
    }

    private static DomainTenantStatus MapToDomainTenantStatus(ProtosTenantStatus status)
    {
        return status switch
        {
            ProtosTenantStatus.Pending => DomainTenantStatus.Pending,
            ProtosTenantStatus.Active => DomainTenantStatus.Active,
            ProtosTenantStatus.Suspended => DomainTenantStatus.Suspended,
            ProtosTenantStatus.Archived => DomainTenantStatus.Archived,
            _ => DomainTenantStatus.Pending
        };
    }

    private static ProtosTenantStatus MapToProtosTenantStatus(DomainTenantStatus status)
    {
        return status switch
        {
            DomainTenantStatus.Pending => ProtosTenantStatus.Pending,
            DomainTenantStatus.Active => ProtosTenantStatus.Active,
            DomainTenantStatus.Suspended => ProtosTenantStatus.Suspended,
            DomainTenantStatus.Archived => ProtosTenantStatus.Archived,
            _ => ProtosTenantStatus.Pending
        };
    }
}

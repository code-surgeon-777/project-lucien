using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ProjectLucien.Api.Controllers;
using ProjectLucien.Api.Interceptors;
using ProjectLucien.Api.Middleware;
using ProjectLucien.Api.Services;
using ProjectLucien.Domain.Entities;
using ProjectLucien.Domain.Ports.Inbound;
using ProjectLucien.Domain.Ports.Outbound;
using ProjectLucien.Domain.UseCases;
using ProjectLucien.Infrastructure;
using ProjectLucien.Infrastructure.Auth0;
using ProjectLucien.Infrastructure.Repositories;
using ProjectLucien.Infrastructure.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddOData(options =>
    {
        options.AddRouteComponents("odata", GetEdmModel());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Project Lucien API",
        Version = "v1",
        Description = "API for managing tenants, including CRUD operations, API key management, and tenant lifecycle operations."
    });

    // Resolve conflicting actions in OData controllers
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// ============================================================
// Step 10.1: Configure full DI wiring
// ============================================================

// Register DbContext (already configured)
builder.Services.AddDbContext<ProjectLucienDbContext>(options =>
    options.UseInMemoryDatabase("ProjectLucienDb"));

// Register repositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

// Register use cases (all 6 inbound port implementations)
builder.Services.AddScoped<ICreateTenantUseCase, CreateTenantUseCase>();
builder.Services.AddScoped<IGetTenantUseCase, GetTenantUseCase>();
builder.Services.AddScoped<IActivateTenantUseCase, ActivateTenantUseCase>();
builder.Services.AddScoped<ISuspendTenantUseCase, SuspendTenantUseCase>();
builder.Services.AddScoped<IArchiveTenantUseCase, ArchiveTenantUseCase>();
builder.Services.AddScoped<IAddApiKeyUseCase, AddApiKeyUseCase>();

// Register outbound services
builder.Services.AddScoped<IAuth0TenantService, Auth0TenantService>(sp =>
{
    var config = builder.Configuration.GetSection("Auth0").Get<Auth0Config>() ?? new Auth0Config();
    return new Auth0TenantService(config);
});
builder.Services.AddScoped<IBITenantService, BITenantService>();

// ============================================================
// Step 10.2 & 10.3: gRPC authentication with interceptor
// ============================================================

// Register gRPC services with authentication interceptor
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<AuthInterceptor>();
});

// ============================================================
// Step 10.5: Configure health checks with database connectivity
// ============================================================

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ProjectLucienDbContext>("database");

var app = builder.Build();

// ============================================================
// Step 10.4: Add API key validation middleware
// ============================================================

app.UseApiKeyValidation();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger/index.html");
        return;
    }
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Lucien API v1"));
}

app.UseAuthorization();

// ============================================================
// Health check endpoints (liveness and readiness probes)
// ============================================================

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow
        }));
    }
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false // Liveness probe - no checks needed
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        }));
    }
});

app.MapControllers();

// Map gRPC endpoint
app.MapGrpcService<InternalLibrarianService>();

app.Run();

static IEdmModel GetEdmModel()
{
    var modelBuilder = new ODataConventionModelBuilder();

    var tenants = modelBuilder.EntitySet<TenantDto>("Tenants");
    tenants.EntityType.HasKey(t => t.Id);

    return modelBuilder.GetEdmModel();
}

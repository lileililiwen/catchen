using System.Security.Claims;
using System.Text;
using Catchen.Affiliates;
using Catchen.Api.Endpoints;
using Catchen.Catalog;
using Catchen.Commerce;
using Catchen.Data;
using Catchen.Documents;
using Catchen.Editorial;
using Catchen.Identity;
using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Catchen.Moderation;
using Catchen.Reporting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCatchenData(builder.Configuration);

// Composition root: one line per module (Agents.md §2.1).
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCatalogModule();
builder.Services.AddEditorialModule();
builder.Services.AddCommerceModule();
builder.Services.AddDocumentsModule();
builder.Services.AddAffiliatesModule();
builder.Services.AddModerationModule();
builder.Services.AddReportingModule();

var identitySection = builder.Configuration.GetSection(IdentityOptions.SectionName);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = identitySection["JwtIssuer"],
            ValidateAudience = true,
            ValidAudience = identitySection["JwtAudience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(identitySection["JwtSecret"]
                    ?? throw new InvalidOperationException("Identity:JwtSecret is required"))),
            ValidateLifetime = true,
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Apply migrations, then seed the initial administrator (empty database only).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Catchen.Seed");
    await IdentitySeeder.EnsureAdminAsync(
        db,
        scope.ServiceProvider.GetRequiredService<IConfiguration>(),
        scope.ServiceProvider.GetRequiredService<IPasswordHasher>(),
        logger);
}

app.MapGet("/healthz", (DatabaseProviderSelector selector) => Results.Ok(new
{
    status = "ok",
    database = selector.Select().ToString(),
}));

app.MapPost("/api/auth/register", async (
    RegisterEndpointRequest request,
    IAccountService accounts,
    HttpContext http,
    CancellationToken cancellationToken) =>
{
    var result = await accounts.RegisterAsync(new RegistrationRequest(
        request.Email,
        request.Password,
        request.Phone,
        request.DeclaredCountryCode,
        request.AgreementVersionAccepted,
        http.Connection.RemoteIpAddress?.ToString(),
        http.Request.Headers.UserAgent.ToString()), cancellationToken);

    return result.Succeeded
        ? Results.Created($"/api/users/{result.UserId}", new { userId = result.UserId })
        : Results.BadRequest(new { violations = result.Violations });
});

app.MapPost("/api/auth/login", async (
    LoginEndpointRequest request,
    IAccountService accounts,
    CancellationToken cancellationToken) =>
{
    var result = await accounts.AuthenticateAsync(request.Email, request.Password, cancellationToken);
    return result.Succeeded
        ? Results.Ok(new { token = result.Token, expiresAtUtc = result.ExpiresAtUtc })
        : Results.Unauthorized();
});

app.MapGet("/api/policy/payment-methods", (IChannelPolicyService channels) => Results.Ok(new
{
    allowed = channels.AllowedPaymentMethods(),
}));

app.MapPost("/api/admin/promotion-channels/approvals", async (
    ApproveChannelRequest request,
    IChannelPolicyService channels,
    ClaimsPrincipal principal,
    CancellationToken cancellationToken) =>
{
    var approverId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("authenticated user id missing"));
    var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    var result = await channels.ApproveChannelAsync(
        request.Channel, request.Kind, approverId, role, cancellationToken);

    if (result.Succeeded)
    {
        return Results.Created($"/api/admin/promotion-channels/approvals/{result.ApprovalId}", new { id = result.ApprovalId });
    }

    return result.Violation == "forbidden_role"
        ? Results.Forbid()
        : Results.UnprocessableEntity(new { violation = result.Violation });
}).RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapGet("/api/admin/promotion-channels/approvals", async (
    IChannelPolicyService channels,
    CancellationToken cancellationToken) =>
    Results.Ok(new { approvals = await channels.ListApprovedAsync(cancellationToken: cancellationToken) }))
    .RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.UseAuthentication();
app.UseAuthorization();

await app.RunAsync();

/// <summary>Exposes the implicit Program class for integration-test hosts.</summary>
public partial class Program
{
}


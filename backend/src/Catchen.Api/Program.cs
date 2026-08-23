using System.Security.Claims;
using System.Text;
using Catchen.Affiliates;
using Catchen.Api.Endpoints;
using Catchen.Catalog;
using Catchen.Catalog.Models;
using Catchen.Catalog.Services;
using Catchen.Commerce;
using Catchen.Data;
using Catchen.Documents;
using Catchen.Editorial;
using Catchen.Editorial.Services;
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Catchen API",
        Version = "v1",
        Description = "Offshore-only cooking platform: recipes, memberships, operations.",
    });

    // Bearer scheme so generated clients attach the JWT on secured calls.
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste the JWT from /api/auth/login.",
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });
});

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
        ? Results.Created($"/api/users/{result.UserId}", new RegisterResponse(result.UserId!.Value))
        : Results.BadRequest(new { violations = result.Violations });
}).Produces<RegisterResponse>(201);

app.MapPost("/api/auth/login", async (
    LoginEndpointRequest request,
    IAccountService accounts,
    CancellationToken cancellationToken) =>
{
    var result = await accounts.AuthenticateAsync(request.Email, request.Password, cancellationToken);
    return result.Succeeded
        ? Results.Ok(new LoginResponse(result.Token!, result.ExpiresAtUtc!.Value))
        : Results.Unauthorized();
}).Produces<LoginResponse>(200);

// ---- Catalog (consumer) -------------------------------------------------

CatalogQuery ReadQuery(string? category, string? difficulty, string? ingredient, string? q)
{
    CuisineCategory? categoryParsed =
        Enum.TryParse<CuisineCategory>(category, ignoreCase: true, out var c) ? c : null;
    RecipeDifficulty? difficultyParsed =
        Enum.TryParse<RecipeDifficulty>(difficulty, ignoreCase: true, out var d) ? d : null;
    return new CatalogQuery(categoryParsed, difficultyParsed, ingredient, q);
}

app.MapGet("/api/catalog/recipes", async (
    ICatalogService catalog,
    string? category,
    string? difficulty,
    string? ingredient,
    string? q,
    CancellationToken ct) =>
{
    var items = await catalog.BrowseAsync(ReadQuery(category, difficulty, ingredient, q), ct);
    return Results.Ok(new { items });
});

app.MapGet("/api/catalog/recipes/{id:guid}", async (
    Guid id,
    ICatalogService catalog,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    Guid? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) is { } raw
        ? Guid.Parse(raw)
        : null;
    var detail = await catalog.GetDetailAsync(id, userId, ct);
    return detail is null ? Results.NotFound() : Results.Ok(detail);
}).AllowAnonymous();

app.MapPost("/api/catalog/recipes/{id:guid}/favorite", async (
    Guid id,
    IFavoritesService favorites,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    return await favorites.AddAsync(userId, id, ct) ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapDelete("/api/catalog/recipes/{id:guid}/favorite", async (
    Guid id,
    IFavoritesService favorites,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    return await favorites.RemoveAsync(userId, id, ct) ? Results.NoContent() : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/api/catalog/favorites", async (
    IFavoritesService favorites,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    return Results.Ok(new { items = await favorites.ListMineAsync(userId, ct) });
}).RequireAuthorization();

app.MapPost("/api/catalog/recipes/{id:guid}/comments", async (
    Guid id,
    CommentRequest request,
    ICommentsService comments,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var result = await comments.AddAsync(userId, id, request.Text, ct);
    return result.Succeeded
        ? Results.Created($"/api/catalog/recipes/{id}/comments", new { id = result.CommentId })
        : Results.UnprocessableEntity(new { violation = result.Violation });
}).RequireAuthorization();

app.MapGet("/api/catalog/recipes/{id:guid}/comments", async (
    Guid id, ICommentsService comments, CancellationToken ct) =>
    Results.Ok(new { comments = await comments.ListVisibleAsync(id, ct) }))
    .AllowAnonymous();

// ---- Editorial workflow (staff) -----------------------------------------

app.MapPost("/api/editorial/drafts", async (
    CreateDraftRequest request,
    IEditorialWorkflowService editorial,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    var result = await editorial.CreateDraftAsync(
        userId, role, request.Content, request.Provenance, request.IsFree, ct);
    return ToWorkflowHttp(result, "/api/editorial/drafts");
}).RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapPut("/api/editorial/drafts/{id:guid}", async (
    Guid id,
    CreateDraftRequest request,
    IEditorialWorkflowService editorial,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var result = await editorial.UpdateDraftAsync(id, userId, request.Content, request.Provenance, request.IsFree, ct);
    return ToWorkflowHttp(result, "/api/editorial/drafts");
}).RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapPost("/api/editorial/drafts/{id:guid}/submit", async (
    Guid id,
    IEditorialWorkflowService editorial,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var result = await editorial.SubmitAsync(id, userId, ct);
    return ToWorkflowHttp(result, "/api/editorial/drafts");
}).RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapPost("/api/admin/drafts/{id:guid}/secondary-review", async (
    Guid id,
    IEditorialWorkflowService editorial,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var result = await editorial.RecordSecondaryReviewAsync(id, userId, ct);
    return ToWorkflowHttp(result, "/api/editorial/drafts");
}).RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapPost("/api/admin/drafts/{id:guid}/publish", async (
    Guid id,
    IEditorialWorkflowService editorial,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    var result = await editorial.PublishAsync(id, userId, role, ct);
    return ToWorkflowHttp(result, "/api/catalog/recipes");
}).RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapPost("/api/admin/recipes/{id:guid}/unpublish", async (
    Guid id,
    IEditorialWorkflowService editorial,
    ClaimsPrincipal principal,
    CancellationToken ct) =>
{
    var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var role = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    var result = await editorial.UnpublishAsync(id, userId, role, ct);
    return ToWorkflowHttp(result, "/api/catalog/recipes");
}).RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

// ---- Moderation (administrator) -----------------------------------------

app.MapPost("/api/admin/comments/{id:guid}/hide", async (
    Guid id,
    ReasonCodeRequest request,
    ICommentsService comments,
    CancellationToken ct) =>
    await comments.HideAsync(id, request.ReasonCode, ct) ? Results.NoContent() : Results.NotFound())
    .RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapPost("/api/admin/users/{id:guid}/block", async (
    Guid id,
    ReasonCodeRequest request,
    ICommentsService comments,
    CancellationToken ct) =>
    await comments.BlockUserAsync(id, request.ReasonCode, ct) ? Results.NoContent() : Results.NotFound())
    .RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

static IResult ToWorkflowHttp(WorkflowResult result, string locationBase)
{
    if (result.Succeeded)
    {
        return Results.Created($"{locationBase}/{result.DraftId}", new
        {
            draftId = result.DraftId,
            publishedRecipeId = result.PublishedRecipeId,
        });
    }

    return result.Violation!.StartsWith("forbidden_role", StringComparison.Ordinal)
        ? Results.Forbid()
        : Results.UnprocessableEntity(new { violation = result.Violation });
}

app.MapGet("/api/policy/payment-methods", (IChannelPolicyService channels) => Results.Ok(
    new PaymentMethodsResponse(channels.AllowedPaymentMethods())))
    .Produces<PaymentMethodsResponse>(200);

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
        return Results.Created(
            $"/api/admin/promotion-channels/approvals/{result.ApprovalId}",
            new ApprovalResponse(result.ApprovalId!.Value));
    }

    return result.Violation == "forbidden_role"
        ? Results.Forbid()
        : Results.UnprocessableEntity(new { violation = result.Violation });
})
.Produces<ApprovalResponse>(201)
.RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.MapGet("/api/admin/promotion-channels/approvals", async (
    IChannelPolicyService channels,
    CancellationToken cancellationToken) => Results.Ok(
        new ApprovedChannelsResponse(await channels.ListApprovedAsync(null, cancellationToken))))
    .Produces<ApprovedChannelsResponse>(200)
    .RequireAuthorization(policy => policy.RequireRole(AppUserRoles.Administrator));

app.UseAuthentication();
app.UseAuthorization();

// OpenAPI contract consumed by the Flutter clients' codegen (task 1.2).
app.UseSwagger(options => options.RouteTemplate = "openapi/{documentName}.json");
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Catchen API v1");
        options.RoutePrefix = "swagger";
    });
}

await app.RunAsync();

/// <summary>Exposes the implicit Program class for integration-test hosts.</summary>
public partial class Program
{
}


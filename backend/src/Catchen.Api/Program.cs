using Catchen.Affiliates;
using Catchen.Catalog;
using Catchen.Commerce;
using Catchen.Data;
using Catchen.Documents;
using Catchen.Editorial;
using Catchen.Identity;
using Catchen.Moderation;
using Catchen.Reporting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCatchenData(builder.Configuration);

// Composition root: one line per module (Agents.md §2.1).
builder.Services.AddIdentityModule();
builder.Services.AddCatalogModule();
builder.Services.AddEditorialModule();
builder.Services.AddCommerceModule();
builder.Services.AddDocumentsModule();
builder.Services.AddAffiliatesModule();
builder.Services.AddModerationModule();
builder.Services.AddReportingModule();

var app = builder.Build();

app.MapGet("/healthz", (DatabaseProviderSelector selector) => Results.Ok(new
{
    status = "ok",
    database = selector.Select().ToString(),
}));

await app.RunAsync();

/// <summary>
/// Exposes the implicit Program class for integration-test hosts
/// (WebApplicationFactory requires a non-static type parameter).
/// </summary>
public partial class Program
{
}

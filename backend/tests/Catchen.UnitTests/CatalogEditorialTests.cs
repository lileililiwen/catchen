using Catchen.Catalog.Models;
using Catchen.Catalog.Services;
using Catchen.Data;
using Catchen.Editorial.Models;
using Catchen.Editorial.Services;
using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace Catchen.UnitTests;

/// <summary>
/// Tasks 2.1–2.4: deterministic recipe validation, the editorial workflow
/// (separation of duties, provenance gate, secondary review, immutable
/// versions), catalog discovery/filters, entitlement-aware detail,
/// favorites, and moderated comments — against SQLite in-memory.
/// </summary>
public sealed class CatalogEditorialTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContext _db;
    private readonly FakeClock _clock = new();
    private readonly RecipeValidator _validator = new();

    public CatalogEditorialTests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    // ---- factories -------------------------------------------------------

    public static TheoryData<string> VaguePhrases =>
        new("a little", "a dash", "a pinch", "to taste", "appropriate amount");

    private static RecipeContent ValidContent(
        string title = "Mapo Tofu",
        string cuisine = "sichuan",
        string difficulty = "medium",
        string? poisonInstruction = null,
        string? poisonIngredientName = null,
        string? badUnit = null)
    {
        var ingredients = new List<IngredientLine>
        {
            new()
            {
                Name = poisonIngredientName ?? "silken tofu",
                Quantity = badUnit is null
                    ? new Quantity { Value = 400, Unit = "g" }
                    : new Quantity { Value = 1, Unit = badUnit },
            },
            new()
            {
                Name = "doubanjiang",
                Quantity = new Quantity { Value = 2, Unit = "tbsp" },
                Substitution = new Substitution
                {
                    Item = "chili bean paste from any Western supermarket",
                    AsianGroceryOnly = true,
                    Note = "Add 1 tsp sugar to balance.",
                },
            },
        };

        return new RecipeContent
        {
            Title = title,
            Cuisine = cuisine,
            Difficulty = difficulty,
            PreviewText = "Classic numbing-spicy tofu in 20 minutes.",
            Ingredients = ingredients,
            Instructions =
            [
                "Cut the tofu into 2 cm cubes.",
                poisonInstruction ?? "Fry doubanjiang in 2 tbsp oil for 30 seconds.",
            ],
            Equipment = ["wok or large skillet", "slotted spoon"],
            CulturalContext = "A Sichuan home classic often served over rice.",
        };
    }

    private static ProvenanceEvidence ValidProvenance(
        bool textAttested = true, bool photoAttested = true)
    {
        return new()
        {
            OriginalTextAttested = textAttested,
            OriginalPhotographyAttested = photoAttested,
            SourceNote = "Developed by our test kitchen.",
        };
    }

    private EditorialWorkflowService Workflow()
    {
        return new(_db, _validator, new AuditWriter(_db, _clock), _clock);
    }

    private static Task<(Guid authorId, Guid reviewerId)> SeedStaffAsync()
    {
        return Task.FromResult((Guid.NewGuid(), Guid.NewGuid()));
    }

    private static Guid AssertId(WorkflowResult result)
    {
        Assert.True(result.Succeeded);
        Assert.NotNull(result.DraftId);
        return result.DraftId.Value;
    }

    // ---- 2.2 deterministic validation ------------------------------------

    [Fact]
    public void Complete_recipe_with_exact_measures_passes_validation()
    {
        var report = _validator.Validate(ValidContent(), ValidProvenance());

        Assert.True(report.IsValid);
        Assert.Empty(report.Violations);
    }

    [Theory]
    [MemberData(nameof(VaguePhrases))]
    public void Vague_quantity_in_instruction_blocks_validation(string phrase)
    {
        var content = ValidContent(poisonInstruction: $"Season with {phrase} salt.");

        var report = _validator.Validate(content, ValidProvenance());

        Assert.False(report.IsValid);
        Assert.Contains(report.Violations, v => v.Contains($"vague_quantity:\"{phrase}\""));
    }

    [Fact]
    public void Vague_quantity_in_ingredient_name_is_identified()
    {
        var content = ValidContent(poisonIngredientName: "salt to taste");

        var report = _validator.Validate(content, ValidProvenance());

        Assert.False(report.IsValid);
        Assert.Contains(report.Violations, v => v.StartsWith("ingredient[0]:name:vague_quantity", StringComparison.Ordinal));
    }

    [Fact]
    public void Non_convertible_unit_blocks_validation()
    {
        var content = ValidContent(badUnit: "splash");

        var report = _validator.Validate(content, ValidProvenance());

        Assert.False(report.IsValid);
        Assert.Contains(report.Violations, v => v.Contains("unit_not_convertible:splash"));
    }

    [Fact]
    public void Missing_provenance_attestations_block_validation()
    {
        var report = _validator.Validate(ValidContent(), ValidProvenance(photoAttested: false));

        Assert.False(report.IsValid);
        Assert.Contains("provenance_original_photography_not_attested", report.Violations);
    }

    [Theory]
    [InlineData("mala_hotpot")]
    [InlineData("extreme")]
    public void Unknown_cuisine_or_difficulty_blocks_validation(string bad)
    {
        var content = ValidContent(cuisine: bad, difficulty: bad);

        var report = _validator.Validate(content, ValidProvenance());

        Assert.False(report.IsValid);
        Assert.Contains(report.Violations, v => v.StartsWith("cuisine_invalid", StringComparison.Ordinal));
        Assert.Contains(report.Violations, v => v.StartsWith("difficulty_invalid", StringComparison.Ordinal));
    }

    // ---- 2.3 editorial workflow ------------------------------------------

    [Fact]
    public async Task Full_workflow_publishes_immutable_version_with_evidence()
    {
        var (authorId, reviewerId) = await SeedStaffAsync();
        var workflow = Workflow();

        var created = await workflow.CreateDraftAsync(
            authorId, AppUserRoles.Administrator, ValidContent(), ValidProvenance(), isFree: true);
        Assert.True(created.Succeeded);

        var submitted = await workflow.SubmitAsync(AssertId(created), authorId);
        Assert.True(submitted.Succeeded);

        var reviewed = await workflow.RecordSecondaryReviewAsync(AssertId(created), reviewerId);
        Assert.True(reviewed.Succeeded);

        var published = await workflow.PublishAsync(AssertId(created), reviewerId, AppUserRoles.Administrator);
        Assert.True(published.Succeeded);
        Assert.NotNull(published.PublishedRecipeId);
        var liveRecipe = (await _db.Set<PublishedRecipe>().ToListAsync()).Single();

        Assert.True(liveRecipe.IsLive);
        Assert.Equal(1, liveRecipe.Version);
        Assert.Equal(authorId, liveRecipe.AuthorUserId);
        Assert.Equal(reviewerId, liveRecipe.ReviewerUserId);

        // Evidence trail: created, submitted, secondary review, published.
        Assert.Equal(4, await _db.Set<AuditEvent>()
            .CountAsync(e => e.Category == "editorial"));
    }

    [Fact]
    public async Task Author_cannot_publish_their_own_draft()
    {
        var (authorId, _) = await SeedStaffAsync();
        var workflow = Workflow();

        var created = await workflow.CreateDraftAsync(
            authorId, AppUserRoles.Administrator, ValidContent(), ValidProvenance(), true);
        await workflow.SubmitAsync(AssertId(created), authorId);
        await workflow.RecordSecondaryReviewAsync(AssertId(created), Guid.NewGuid());

        var result = await workflow.PublishAsync(
            AssertId(created), authorId, AppUserRoles.Administrator);

        Assert.False(result.Succeeded);
        Assert.Equal("author_cannot_publish", result.Violation);
    }

    [Fact]
    public async Task Publication_is_blocked_without_secondary_review()
    {
        var (authorId, reviewerId) = await SeedStaffAsync();
        var workflow = Workflow();

        var created = await workflow.CreateDraftAsync(
            authorId, AppUserRoles.Administrator, ValidContent(), ValidProvenance(), true);
        await workflow.SubmitAsync(AssertId(created), authorId);

        var result = await workflow.PublishAsync(
            AssertId(created), reviewerId, AppUserRoles.Administrator);

        Assert.False(result.Succeeded);
        Assert.Equal("secondary_review_missing", result.Violation);
        Assert.Empty(await _db.Set<PublishedRecipe>().ToListAsync());
    }

    [Fact]
    public async Task Submission_with_vague_measure_is_blocked_and_reported()
    {
        var (authorId, _) = await SeedStaffAsync();
        var workflow = Workflow();

        var created = await workflow.CreateDraftAsync(
            authorId, AppUserRoles.Administrator, ValidContent(), ValidProvenance(), true);
        await workflow.UpdateDraftAsync(
            AssertId(created), authorId,
            ValidContent(poisonInstruction: "Add a pinch of sugar."),
            ValidProvenance(), true);

        var submitted = await workflow.SubmitAsync(AssertId(created), authorId);

        Assert.False(submitted.Succeeded);
        Assert.StartsWith("validation_failed:", submitted.Violation);

        var draft = await _db.Set<RecipeDraft>().SingleAsync();
        Assert.Equal(DraftStatus.ChangesRequested, draft.Status);
        Assert.Contains("vague_quantity", draft.ValidationReportJson);
    }

    [Fact]
    public async Task Republishing_the_same_recipe_increments_the_version()
    {
        var (authorId, reviewerId) = await SeedStaffAsync();
        var workflow = Workflow();

        var created = await workflow.CreateDraftAsync(
            authorId, AppUserRoles.Administrator, ValidContent(), ValidProvenance(), true);
        await workflow.SubmitAsync(AssertId(created), authorId);
        await workflow.RecordSecondaryReviewAsync(AssertId(created), reviewerId);
        await workflow.PublishAsync(AssertId(created), reviewerId, AppUserRoles.Administrator);

        await workflow.UnpublishAsync(AssertId(created), reviewerId, AppUserRoles.Administrator);
        await workflow.SubmitAsync(AssertId(created), authorId);
        await workflow.RecordSecondaryReviewAsync(AssertId(created), reviewerId);
        var second = await workflow.PublishAsync(
            AssertId(created), reviewerId, AppUserRoles.Administrator);

        Assert.True(second.Succeeded);
        var rowCount = await _db.Set<PublishedRecipe>().CountAsync(r => r.RecipeId == AssertId(created));
        Assert.True(rowCount == 2, $"rowCount={rowCount}");
        var rows = await _db.Set<PublishedRecipe>().ToListAsync();
        Assert.Contains(rows, r => r.Version == 1 && !r.IsLive);
        Assert.Contains(rows, r => r.Version == 2 && r.IsLive);
    }

    // ---- 2.4 catalog discovery, entitlements, favorites, comments ---------

    private async Task<(Guid authorId, Guid reviewerId, Guid freeRecipeId, Guid premiumRecipeId)>
        PublishTwoRecipesAsync()
    {
        var (authorId, reviewerId) = await SeedStaffAsync();
        var workflow = Workflow();

        foreach (var (content, isFree) in new[]
                 {
                     (ValidContent(title: "Free Stir Fry"), true),
                     (ValidContent(title: "Premium Banquet"), false),
                 })
        {
            var created = await workflow.CreateDraftAsync(authorId, AppUserRoles.Administrator, content, ValidProvenance(), isFree);
            await workflow.SubmitAsync(AssertId(created), authorId);
            await workflow.RecordSecondaryReviewAsync(AssertId(created), reviewerId);
            await workflow.PublishAsync(AssertId(created), reviewerId, AppUserRoles.Administrator);
        }

        var freeId = await _db.Set<PublishedRecipe>()
            .Where(r => r.IsFree && r.IsLive).Select(r => r.RecipeId).SingleAsync();
        var premiumId = await _db.Set<PublishedRecipe>()
            .Where(r => !r.IsFree && r.IsLive).Select(r => r.RecipeId).SingleAsync();
        return (authorId, reviewerId, freeId, premiumId);
    }

    [Fact]
    public async Task Combined_filters_return_only_matching_published_recipes()
    {
        await PublishTwoRecipesAsync();
        var catalog = new CatalogService(_db, new NoEntitlementProvider());

        var all = await catalog.BrowseAsync(new CatalogQuery(null, null, null, null));
        Assert.Equal(2, all.Count);

        var filtered = await catalog.BrowseAsync(
            new CatalogQuery(CuisineCategory.Sichuan, RecipeDifficulty.Medium, "tofu", null));
        Assert.Equal(2, filtered.Count); // both seeded recipes match these axes

        var none = await catalog.BrowseAsync(
            new CatalogQuery(CuisineCategory.Cantonese, null, null, null));
        Assert.Empty(none);

        var search = await catalog.BrowseAsync(new CatalogQuery(null, null, null, "banquet"));
        Assert.Single(search);
        Assert.Equal("Premium Banquet", search[0].Title);
    }

    [Fact]
    public async Task Locked_premium_recipe_returns_preview_without_content()
    {
        var (_, _, _, premiumId) = await PublishTwoRecipesAsync();
        var catalog = new CatalogService(_db, new NoEntitlementProvider());

        var anonymous = await catalog.GetDetailAsync(premiumId, null);
        Assert.NotNull(anonymous);
        Assert.Null(anonymous.ContentJson);
        Assert.NotEmpty(anonymous.PurchaseOptions);
        Assert.NotEqual(string.Empty, anonymous.Summary.PreviewText);

        var entitledCatalog = new CatalogService(_db, new EntitledProvider());
        var entitled = await entitledCatalog.GetDetailAsync(premiumId, Guid.NewGuid());
        Assert.NotNull(entitled!.ContentJson);
    }

    private sealed class EntitledProvider : IEntitlementProvider
    {
        public Task<bool> HasFullAccessAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }
    }

    [Fact]
    public async Task Free_recipe_serves_full_content_to_anonymous_users()
    {
        var (_, _, freeId, _) = await PublishTwoRecipesAsync();
        var catalog = new CatalogService(_db, new NoEntitlementProvider());

        var detail = await catalog.GetDetailAsync(freeId, null);

        Assert.NotNull(detail!.ContentJson);
        Assert.Empty(detail.PurchaseOptions);
    }

    [Fact]
    public async Task Favorites_persist_and_remove_for_the_user()
    {
        var (_, consumerId, freeId, _) = await PublishTwoRecipesAsync();
        var favorites = new FavoritesService(_db, _clock);

        Assert.True(await favorites.AddAsync(consumerId, freeId));
        Assert.Single(await favorites.ListMineAsync(consumerId));

        // Idempotent re-add keeps a single row.
        Assert.True(await favorites.AddAsync(consumerId, freeId));
        Assert.Single(await _db.Set<RecipeFavorite>()
            .Where(f => f.UserId == consumerId)
            .ToListAsync());

        Assert.True(await favorites.RemoveAsync(consumerId, freeId));
        Assert.Empty(await favorites.ListMineAsync(consumerId));
    }

    [Fact]
    public async Task Hidden_comment_is_retained_but_not_publicly_visible()
    {
        var (_, consumerId, freeId, _) = await PublishTwoRecipesAsync();
        var comments = new CommentsService(_db, _clock);

        var added = await comments.AddAsync(consumerId, freeId, "Great weeknight dish!");
        Assert.True(added.Succeeded);
        var commentId = added.CommentId!.Value;

        Assert.True(await comments.HideAsync(commentId, "off_topic"));

        Assert.Empty(await comments.ListVisibleAsync(freeId));           // not public…
        Assert.Single(await _db.Set<RecipeComment>().ToListAsync());      // …but retained for audit
    }

    [Fact]
    public async Task Blocked_user_cannot_submit_new_comments()
    {
        var (_, consumerId, freeId, _) = await PublishTwoRecipesAsync();
        var comments = new CommentsService(_db, _clock);

        Assert.True(await comments.BlockUserAsync(consumerId, "abuse_policy"));

        var result = await comments.AddAsync(consumerId, freeId, "hello again");
        Assert.False(result.Succeeded);
        Assert.Equal("user_blocked", result.Violation);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

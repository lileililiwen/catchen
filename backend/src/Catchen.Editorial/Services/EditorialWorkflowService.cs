using Catchen.Catalog.Models;
using Catchen.Editorial.Models;
using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Editorial.Services;

public sealed record WorkflowResult(Guid? DraftId, Guid? PublishedRecipeId, string? Violation)
{
    public bool Succeeded => Violation is null;

    public static WorkflowResult Ok(Guid draftId, Guid? publishedRecipeId = null)
    {
        return new(draftId, publishedRecipeId, null);
    }

    public static WorkflowResult Rejected(string violation)
    {
        return new(null, null, violation);
    }
}

/// <summary>
/// Editorial workflow (task 2.3): create → submit (validated) → secondary
/// usability review → approve &amp; publish (immutable catalog version) →
/// unpublish. Author and final reviewer MUST differ; publication requires
/// provenance attestations and a recorded secondary review.
/// </summary>
public interface IEditorialWorkflowService
{
    Task<WorkflowResult> CreateDraftAsync(
        Guid authorUserId, string authorRole, RecipeContent content, ProvenanceEvidence provenance,
        bool isFree, CancellationToken cancellationToken = default);

    Task<WorkflowResult> UpdateDraftAsync(
        Guid draftId, Guid actorUserId, RecipeContent content, ProvenanceEvidence provenance,
        bool isFree, CancellationToken cancellationToken = default);

    /// <summary>Runs deterministic validation; submission without a clean report is rejected.</summary>
    Task<WorkflowResult> SubmitAsync(Guid draftId, Guid actorUserId, CancellationToken cancellationToken = default);

    Task<WorkflowResult> RecordSecondaryReviewAsync(
        Guid draftId, Guid reviewerUserId, CancellationToken cancellationToken = default);

    Task<WorkflowResult> PublishAsync(
        Guid draftId, Guid reviewerUserId, string reviewerRole, CancellationToken cancellationToken = default);

    Task<WorkflowResult> UnpublishAsync(
        Guid recipeId, Guid actorUserId, string actorRole, CancellationToken cancellationToken = default);

    /// <summary>Publication-status counters for ops reporting (task 4.2).</summary>
    Task<IReadOnlyDictionary<string, int>> StatusCountsAsync(CancellationToken cancellationToken = default);
}

public sealed class EditorialWorkflowService(
    DbContext db,
    IRecipeValidator validator,
    IAuditWriter audit,
    TimeProvider clock) : IEditorialWorkflowService
{
    public async Task<WorkflowResult> CreateDraftAsync(
        Guid authorUserId, string authorRole, RecipeContent content, ProvenanceEvidence provenance,
        bool isFree, CancellationToken cancellationToken = default)
    {
        if (!IsStaff(authorRole))
        {
            return WorkflowResult.Rejected("forbidden_role");
        }

        // Drafts may be work-in-progress: the deterministic validation gate
        // runs at submission, not while authoring.
        var now = clock.GetUtcNow();
        var draft = new RecipeDraft
        {
            Id = Guid.NewGuid(),
            Title = content.Title,
            Cuisine = content.Cuisine.Trim().ToLowerInvariant(),
            Difficulty = content.Difficulty.Trim().ToLowerInvariant(),
            IsFree = isFree,
            ContentJson = RecipeContentJson.Serialize(content),
            ProvenanceJson = RecipeProvenanceJson.Serialize(provenance),
            Status = DraftStatus.Draft,
            AuthorUserId = authorUserId,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };

        db.Set<RecipeDraft>().Add(draft);
        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync("editorial", "draft.created", authorUserId,
            "RecipeDraft", draft.Id.ToString(), new { title = draft.Title }, cancellationToken);

        return WorkflowResult.Ok(draft.Id);
    }

    public async Task<WorkflowResult> UpdateDraftAsync(
        Guid draftId, Guid actorUserId, RecipeContent content, ProvenanceEvidence provenance,
        bool isFree, CancellationToken cancellationToken = default)
    {
        var draft = await FindDraft(draftId, cancellationToken);
        if (draft is null)
        {
            return WorkflowResult.Rejected("draft_not_found");
        }

        if (draft.AuthorUserId != actorUserId)
        {
            return WorkflowResult.Rejected("not_author");
        }

        if (draft.Status is not (DraftStatus.Draft or DraftStatus.ChangesRequested))
        {
            return WorkflowResult.Rejected("status_not_editable");
        }

        // Work-in-progress saves skip the validation gate (see CreateDraft).
        draft.Title = content.Title;
        draft.Cuisine = content.Cuisine.Trim().ToLowerInvariant();
        draft.Difficulty = content.Difficulty.Trim().ToLowerInvariant();
        draft.IsFree = isFree;
        draft.ContentJson = RecipeContentJson.Serialize(content);
        draft.ProvenanceJson = RecipeProvenanceJson.Serialize(provenance);
        draft.UpdatedAtUtc = clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync("editorial", "draft.updated", actorUserId,
            "RecipeDraft", draft.Id.ToString(), new { }, cancellationToken);

        return WorkflowResult.Ok(draft.Id);
    }

    public async Task<WorkflowResult> SubmitAsync(Guid draftId, Guid actorUserId, CancellationToken cancellationToken = default)
    {
        var draft = await FindDraft(draftId, cancellationToken);
        if (draft is null)
        {
            return WorkflowResult.Rejected("draft_not_found");
        }

        if (draft.AuthorUserId != actorUserId)
        {
            return WorkflowResult.Rejected("not_author");
        }

        // Unpublished recipes re-enter review for their next version.
        if (draft.Status
            is not (DraftStatus.Draft or DraftStatus.ChangesRequested or DraftStatus.Unpublished))
        {
            return WorkflowResult.Rejected("status_not_submittable");
        }

        var content = RecipeContentJson.TryParse(draft.ContentJson);
        var provenance = RecipeProvenanceJson.TryParse(draft.ProvenanceJson);
        var report = validator.Validate(content, provenance);
        draft.ValidationReportJson = System.Text.Json.JsonSerializer.Serialize(report);

        if (!report.IsValid)
        {
            draft.Status = DraftStatus.ChangesRequested;
            await db.SaveChangesAsync(cancellationToken);
            await audit.WriteAsync("editorial", "submit.blocked", actorUserId,
                "RecipeDraft", draft.Id.ToString(),
                new { violations = report.Violations }, cancellationToken);
            return WorkflowResult.Rejected("validation_failed:" + string.Join(",", report.Violations));
        }

        draft.Status = DraftStatus.Submitted;
        draft.UpdatedAtUtc = clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync("editorial", "draft.submitted", actorUserId,
            "RecipeDraft", draft.Id.ToString(), new { }, cancellationToken);

        return WorkflowResult.Ok(draft.Id);
    }

    public async Task<WorkflowResult> RecordSecondaryReviewAsync(
        Guid draftId, Guid reviewerUserId, CancellationToken cancellationToken = default)
    {
        var draft = await FindDraft(draftId, cancellationToken);
        if (draft is null)
        {
            return WorkflowResult.Rejected("draft_not_found");
        }

        if (draft.Status != DraftStatus.Submitted)
        {
            return WorkflowResult.Rejected("status_not_under_review");
        }

        if (draft.AuthorUserId == reviewerUserId)
        {
            return WorkflowResult.Rejected("author_cannot_review");
        }

        draft.SecondaryReviewAtUtc = clock.GetUtcNow();
        draft.SecondaryReviewerUserId = reviewerUserId;
        draft.UpdatedAtUtc = clock.GetUtcNow();
        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync("editorial", "secondary_review.recorded", reviewerUserId,
            "RecipeDraft", draft.Id.ToString(), new { }, cancellationToken);

        return WorkflowResult.Ok(draft.Id);
    }

    public async Task<WorkflowResult> PublishAsync(
        Guid draftId, Guid reviewerUserId, string reviewerRole, CancellationToken cancellationToken = default)
    {
        var draft = await FindDraft(draftId, cancellationToken);
        if (draft is null)
        {
            return WorkflowResult.Rejected("draft_not_found");
        }

        if (!IsStaff(reviewerRole))
        {
            return WorkflowResult.Rejected("forbidden_role");
        }

        if (draft.Status != DraftStatus.Submitted)
        {
            return WorkflowResult.Rejected("status_not_submitted");
        }

        // Separation of duties: the author can never be their own approver.
        if (draft.AuthorUserId == reviewerUserId)
        {
            return WorkflowResult.Rejected("author_cannot_publish");
        }

        // Copyright evidence gate.
        var provenance = RecipeProvenanceJson.TryParse(draft.ProvenanceJson);
        if (provenance is null
            || !provenance.OriginalTextAttested
            || !provenance.OriginalPhotographyAttested)
        {
            await audit.WriteAsync("editorial", "publish.blocked_provenance", reviewerUserId,
                "RecipeDraft", draft.Id.ToString(), new { }, cancellationToken);
            return WorkflowResult.Rejected("provenance_evidence_missing");
        }

        // Secondary usability review gate.
        if (draft.SecondaryReviewAtUtc is null)
        {
            return WorkflowResult.Rejected("secondary_review_missing");
        }

        var lastVersion = await db.Set<PublishedRecipe>()
            .Where(r => r.RecipeId == draft.Id)
            .OrderByDescending(r => r.Version)
            .Select(r => (int?)r.Version)
            .FirstOrDefaultAsync(cancellationToken);

        var version = (lastVersion ?? 0) + 1;
        var now = clock.GetUtcNow();

        var published = new PublishedRecipe
        {
            Id = Guid.NewGuid(),
            RecipeId = draft.Id,
            Version = version,
            Title = draft.Title,
            Cuisine = Enum.Parse<CuisineCategory>(draft.Cuisine, ignoreCase: true),
            Difficulty = Enum.Parse<RecipeDifficulty>(draft.Difficulty, ignoreCase: true),
            PreviewText = RecipeContentJson.TryParse(draft.ContentJson)?.PreviewText ?? string.Empty,
            ContentJson = draft.ContentJson,
            IngredientIndex = string.Join(',',
                RecipeContentJson.TryParse(draft.ContentJson)?.IngredientNames
                    .Select(n => n.ToLowerInvariant()) ?? []),
            IsFree = draft.IsFree,
            IsLive = true,
            AuthorUserId = draft.AuthorUserId,
            ReviewerUserId = reviewerUserId,
            PublishedAtUtc = now,
        };

        db.Set<PublishedRecipe>().Add(published);

        draft.Status = DraftStatus.Published;
        draft.PublishedRecipeId = published.Id;
        draft.PublishedVersion = version;
        draft.ReviewerUserId = reviewerUserId;
        draft.UpdatedAtUtc = now;
        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync("editorial", "recipe.published", reviewerUserId,
            "PublishedRecipe", published.Id.ToString(),
            new { recipeId = published.RecipeId, version, author = draft.AuthorUserId }, cancellationToken);

        return WorkflowResult.Ok(draft.Id, published.Id);
    }

    public async Task<WorkflowResult> UnpublishAsync(
        Guid recipeId, Guid actorUserId, string actorRole, CancellationToken cancellationToken = default)
    {
        if (!IsStaff(actorRole))
        {
            return WorkflowResult.Rejected("forbidden_role");
        }

        var live = await db.Set<PublishedRecipe>()
            .Where(r => r.RecipeId == recipeId && r.IsLive)
            .ToListAsync(cancellationToken);
        if (live.Count == 0)
        {
            return WorkflowResult.Rejected("not_published");
        }

        foreach (var row in live)
        {
            row.IsLive = false;
        }

        var draft = await db.Set<RecipeDraft>().SingleOrDefaultAsync(d => d.Id == recipeId, cancellationToken);
        if (draft is not null && draft.Status == DraftStatus.Published)
        {
            draft.Status = DraftStatus.Unpublished;
            draft.UpdatedAtUtc = clock.GetUtcNow();
        }

        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync("editorial", "recipe.unpublished", actorUserId,
            "PublishedRecipe", recipeId.ToString(), new { versions = live.Count }, cancellationToken);

        return WorkflowResult.Ok(recipeId);
    }

    public async Task<IReadOnlyDictionary<string, int>> StatusCountsAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.Set<RecipeDraft>().AsNoTracking().ToListAsync(cancellationToken);
        return rows
            .GroupBy(d => d.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private async Task<RecipeDraft?> FindDraft(Guid id, CancellationToken ct)
    {
        return await db.Set<RecipeDraft>().SingleOrDefaultAsync(d => d.Id == id, ct);
    }

    private static bool IsStaff(string role)
    {
        return role == AppUserRoles.Administrator;
    }
}

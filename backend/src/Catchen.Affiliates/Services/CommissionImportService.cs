using Catchen.Affiliates.Models;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Affiliates.Services;

public sealed record CommissionImportRow(
    string ExternalRowId,
    string MerchantSlug,
    long AmountMinorUnits,
    string Currency);

public sealed record CommissionImportReport(
    int Accepted,
    int Duplicates,
    int Rejected,
    IReadOnlyList<string> Rejections);

/// <summary>
/// Provider commission statement import (task 4.1): validates, deduplicates
/// on (provider, externalRowId), and reports accepted/duplicate/rejected rows.
/// </summary>
public interface ICommissionImportService
{
    Task<CommissionImportReport> ImportAsync(
        string provider, IReadOnlyList<CommissionImportRow> rows,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CommissionStatementRow>> AcceptedRowsAsync(
        string? merchantSlug = null, CancellationToken cancellationToken = default);
}

public sealed class CommissionImportService(DbContext db, TimeProvider clock) : ICommissionImportService
{
    private static readonly HashSet<string> _knownCurrencies = new(StringComparer.Ordinal)
    {
        "USD", "EUR", "GBP", "CAD", "AUD",
    };

    public async Task<CommissionImportReport> ImportAsync(
        string provider, IReadOnlyList<CommissionImportRow> rows,
        CancellationToken cancellationToken = default)
    {
        var normalizedProvider = provider?.Trim().ToLowerInvariant() ?? string.Empty;
        var accepted = 0;
        var duplicates = 0;
        var rejected = 0;
        var rejections = new List<string>();
        var seenInBatch = new HashSet<string>(StringComparer.Ordinal);

        foreach (var row in rows)
        {
            var externalId = row.ExternalRowId?.Trim() ?? string.Empty;
            var slug = row.MerchantSlug?.Trim().ToLowerInvariant() ?? string.Empty;

            if (externalId.Length == 0)
            {
                rejected++;
                rejections.Add("row_id_missing");
                continue;
            }

            if (row.AmountMinorUnits <= 0 || !_knownCurrencies.Contains(row.Currency ?? string.Empty))
            {
                rejected++;
                rejections.Add($"{externalId}:invalid_amount_or_currency");
                continue;
            }

            var merchantKnown = await db.Set<AffiliateMerchant>().AnyAsync(
                m => m.Slug == slug, cancellationToken);
            if (!merchantKnown)
            {
                rejected++;
                rejections.Add($"{externalId}:unknown_merchant:{slug}");
                continue;
            }

            // Deduplicate against the database AND earlier rows of this same
            // batch (in-memory additions are not visible to AnyAsync yet).
            if (!seenInBatch.Add($"{normalizedProvider}|{externalId}"))
            {
                duplicates++;
                continue;
            }

            var duplicate = await db.Set<CommissionStatementRow>().AnyAsync(
                r => r.Provider == normalizedProvider && r.ExternalRowId == externalId,
                cancellationToken);
            if (duplicate)
            {
                duplicates++;
                continue;
            }

            db.Set<CommissionStatementRow>().Add(new CommissionStatementRow
            {
                Id = Guid.NewGuid(),
                Provider = normalizedProvider,
                ExternalRowId = externalId,
                MerchantSlug = slug,
                AmountMinorUnits = row.AmountMinorUnits,
                Currency = row.Currency!.Trim().ToUpperInvariant(),
                Status = CommissionRowStatus.Accepted,
                ImportedAtUtc = clock.GetUtcNow(),
            });
            accepted++;
        }

        if (accepted > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
        }

        return new CommissionImportReport(accepted, duplicates, rejected, rejections);
    }

    public async Task<IReadOnlyList<CommissionStatementRow>> AcceptedRowsAsync(
        string? merchantSlug = null, CancellationToken cancellationToken = default)
    {
        var query = db.Set<CommissionStatementRow>().AsNoTracking();
        var normalizedSlug = merchantSlug?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(normalizedSlug))
        {
            query = query.Where(r => r.MerchantSlug == normalizedSlug);
        }

        var rows = await query.ToListAsync(cancellationToken);

        // In-memory ordering for provider portability (see CatalogService).
        return rows.OrderBy(r => r.ImportedAtUtc).ToList();
    }
}

using Catchen.Catalog.Models;
using Catchen.Commerce.Models;
using Catchen.Commerce.Services;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Commerce.Services;

public sealed record OrderReportRow(
    Guid OrderId,
    string ProductKind,
    string Status,
    string Provider,
    long AmountMinorUnits,
    string Currency,
    DateTimeOffset CreatedAtUtc);

public sealed record OrderReport(
    int TotalOrders,
    long PaidAmountMinorUnits,
    string Currency,
    IReadOnlyList<OrderReportRow> Rows);

/// <summary>
/// Administrative order reporting (task 3.4): reconciled orders filterable by
/// period, provider, currency and status.
/// </summary>
public interface IOrderReportingService
{
    Task<OrderReport> ReportAsync(
        DateTimeOffset? fromUtc, DateTimeOffset? toUtc,
        string? provider, string? currency, OrderStatus? status,
        CancellationToken cancellationToken = default);
}

public sealed class OrderReportingService(DbContext db) : IOrderReportingService
{
    public async Task<OrderReport> ReportAsync(
        DateTimeOffset? fromUtc, DateTimeOffset? toUtc,
        string? provider, string? currency, OrderStatus? status,
        CancellationToken cancellationToken = default)
    {
        var rows = db.Set<Order>().AsNoTracking();

        if (fromUtc is DateTimeOffset from)
        {
            rows = rows.Where(o => o.CreatedAtUtc >= from);
        }

        if (toUtc is DateTimeOffset to)
        {
            rows = rows.Where(o => o.CreatedAtUtc <= to);
        }

        var normalizedProvider = provider?.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(normalizedProvider))
        {
            rows = rows.Where(o => o.Provider == normalizedProvider);
        }

        var normalizedCurrency = currency?.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(normalizedCurrency))
        {
            rows = rows.Where(o => o.Currency == normalizedCurrency);
        }

        if (status is OrderStatus orderStatus)
        {
            rows = rows.Where(o => o.Status == orderStatus);
        }

        var materialized = await rows.ToListAsync(cancellationToken);

        // In-memory ordering for provider portability (see CatalogService).
        var list = materialized
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new OrderReportRow(
                o.Id,
                o.ProductKind.ToString(),
                o.Status.ToString(),
                o.Provider,
                o.AmountMinorUnits,
                o.Currency,
                o.CreatedAtUtc))
            .ToList();

        // Totals in memory: single-currency Phase 1 (USD); multi-currency
        // breakdowns arrive with the reporting module in a later phase.
        var paidMinorUnits = list
            .Where(r => r.Status == nameof(OrderStatus.Paid) && r.Currency == "USD")
            .Sum(r => r.AmountMinorUnits);

        return new OrderReport(list.Count, paidMinorUnits, "USD", list);
    }
}

using Catchen.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Catchen.Identity.Services;

/// <summary>
/// Privacy-minimization retention job: purges audit events older than the
/// configured retention window. Runs once per day; the purge itself is
/// unit-tested independently of the loop.
/// </summary>
public sealed class AuditRetentionWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<IdentityOptions> options,
    TimeProvider clock,
    ILogger<AuditRetentionWorker> logger) : BackgroundService
{
    internal static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        do
        {
            try
            {
                await PurgeAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.PurgeFailed(exception);
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }

    /// <summary>Deletes audit events older than the retention window. Internal for testing.</summary>
    internal async Task<int> PurgeAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DbContext>();
        var cutoff = clock.GetUtcNow().AddDays(-options.Value.AuditRetentionDays);

        var deleted = await db.Set<AuditEvent>()
            .Where(e => e.OccurredAtUtc < cutoff)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted > 0)
        {
            logger.Purged(deleted, cutoff);
        }

        return deleted;
    }
}

internal static partial class RetentionLogging
{
    [LoggerMessage(Level = LogLevel.Error, Message = "audit retention purge failed; will retry next interval")]
    public static partial void PurgeFailed(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "audit retention purged {Count} events older than {Cutoff:O}")]
    public static partial void Purged(this ILogger logger, int count, DateTimeOffset cutoff);
}

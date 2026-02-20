using Microsoft.Extensions.Caching.Memory;
using PolymarketDashboard.Core.Interfaces;
using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Api.Services;

/// <summary>
/// Background service that proactively refreshes the markets cache every 30 seconds,
/// ensuring the controller always serves near-fresh data without blocking callers.
/// </summary>
public sealed class MarketPollingService : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MarketPollingService> _logger;

    public MarketPollingService(
        IServiceScopeFactory scopeFactory,
        IMemoryCache cache,
        ILogger<MarketPollingService> logger)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Market polling service started");

        // Perform an immediate fetch so the cache is warm on first request.
        await RefreshCacheAsync(stoppingToken);

        using var timer = new PeriodicTimer(PollingInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await RefreshCacheAsync(stoppingToken);
    }

    private async Task RefreshCacheAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IGammaApiService>();
            IReadOnlyList<Market> markets = await service.GetActiveMarketsAsync(ct);

            _cache.Set(CacheKeys.ActiveMarkets, markets, TimeSpan.FromSeconds(30));
            _logger.LogInformation("Market cache refreshed — {Count} markets loaded", markets.Count);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing market cache");
        }
    }
}

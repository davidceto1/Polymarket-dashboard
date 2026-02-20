using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PolymarketDashboard.Api.Services;
using PolymarketDashboard.Core.Interfaces;
using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MarketsController : ControllerBase
{
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromSeconds(30);

    private readonly IGammaApiService _gammaApiService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MarketsController> _logger;

    public MarketsController(
        IGammaApiService gammaApiService,
        IMemoryCache cache,
        ILogger<MarketsController> logger)
    {
        _gammaApiService = gammaApiService;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Returns the current list of active Polymarket markets.
    /// Results are served from a 30-second memory cache to avoid
    /// hammering the upstream Gamma API.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Market>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMarkets(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(CacheKeys.ActiveMarkets, out IReadOnlyList<Market>? cached) && cached is not null)
        {
            _logger.LogDebug("Serving {Count} markets from cache", cached.Count);
            return Ok(cached);
        }

        _logger.LogInformation("Cache miss — fetching markets from Gamma API");
        var markets = await _gammaApiService.GetActiveMarketsAsync(cancellationToken);
        _cache.Set(CacheKeys.ActiveMarkets, markets, CacheExpiry);

        return Ok(markets);
    }
}

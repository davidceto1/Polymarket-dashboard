using Microsoft.AspNetCore.Mvc;
using PolymarketDashboard.Core.Interfaces;
using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Api.Controllers;

[ApiController]
[Route("api/market-detail")]
public sealed class MarketDetailController : ControllerBase
{
    private readonly IMarketDetailService _detailService;
    private readonly IGammaApiService _gammaService;

    public MarketDetailController(IMarketDetailService detailService, IGammaApiService gammaService)
    {
        _detailService = detailService;
        _gammaService  = gammaService;
    }

    /// <summary>
    /// Fetches a single market by conditionId — used when a portfolio position's market
    /// is not already in the cached active-markets list.
    /// </summary>
    [HttpGet("market")]
    [ProducesResponseType(typeof(Market), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMarket(
        [FromQuery] string? conditionId,
        [FromQuery] string? slug,
        CancellationToken cancellationToken)
    {
        Market? market = null;

        if (!string.IsNullOrWhiteSpace(slug))
        {
            market = await _gammaService.GetMarketBySlugAsync(slug, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(conditionId))
        {
            market = await _gammaService.GetMarketByConditionIdAsync(conditionId, cancellationToken);
        }
        else
        {
            return BadRequest(new { error = "Either slug or conditionId is required." });
        }

        if (market is null) return NotFound();
        return Ok(market);
    }

    /// <summary>
    /// Returns price history for a market (proxied from CLOB API).
    /// </summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyList<PricePoint>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHistory(
        [FromQuery] string tokenId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tokenId))
            return BadRequest(new { error = "tokenId is required." });

        var history = await _detailService.GetPriceHistoryAsync(tokenId, cancellationToken);
        return Ok(history);
    }

    /// <summary>
    /// Returns the live order book for a CLOB token (proxied from CLOB API).
    /// </summary>
    [HttpGet("orderbook")]
    [ProducesResponseType(typeof(OrderBook), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrderBook(
        [FromQuery] string tokenId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tokenId))
            return BadRequest(new { error = "tokenId is required." });

        var book = await _detailService.GetOrderBookAsync(tokenId, cancellationToken);
        return Ok(book);
    }
}

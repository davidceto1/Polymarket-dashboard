using Microsoft.AspNetCore.Mvc;
using PolymarketDashboard.Core.Interfaces;
using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PortfolioController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;
    private readonly IConfiguration _config;
    private readonly ILogger<PortfolioController> _logger;

    public PortfolioController(
        IPortfolioService portfolioService,
        IConfiguration config,
        ILogger<PortfolioController> logger)
    {
        _portfolioService = portfolioService;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Returns open positions for the configured wallet address.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Position>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPortfolio(CancellationToken cancellationToken)
    {
        var wallet = _config["Polymarket:WalletAddress"];

        if (string.IsNullOrWhiteSpace(wallet))
            return BadRequest(new { error = "No wallet address configured. Set Polymarket:WalletAddress in appsettings.json." });

        _logger.LogInformation("Fetching positions for wallet {Wallet}", wallet);
        var positions = await _portfolioService.GetPositionsAsync(wallet, cancellationToken);
        return Ok(positions);
    }
}

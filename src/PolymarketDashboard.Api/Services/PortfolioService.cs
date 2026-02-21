using System.Text.Json;
using PolymarketDashboard.Core.Interfaces;
using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Api.Services;

public sealed class PortfolioService : IPortfolioService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<PortfolioService> _logger;

    public PortfolioService(HttpClient httpClient, ILogger<PortfolioService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Position>> GetPositionsAsync(
        string walletAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"/positions?user={Uri.EscapeDataString(walletAddress)}&sizeThreshold=.1";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var positions = await response.Content
                .ReadFromJsonAsync<List<Position>>(JsonOptions, cancellationToken);

            return positions ?? [];
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch positions for wallet {Wallet}", walletAddress);
            return [];
        }
    }
}

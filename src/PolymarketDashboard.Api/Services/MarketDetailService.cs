using System.Text.Json;
using System.Text.Json.Serialization;
using PolymarketDashboard.Core.Interfaces;
using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Api.Services;

public sealed class MarketDetailService : IMarketDetailService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<MarketDetailService> _logger;

    public MarketDetailService(HttpClient httpClient, ILogger<MarketDetailService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PricePoint>> GetPriceHistoryAsync(
        string conditionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"/prices-history?market={Uri.EscapeDataString(conditionId)}&interval=max&fidelity=60";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var wrapper = await response.Content
                .ReadFromJsonAsync<PriceHistoryWrapper>(JsonOptions, cancellationToken);

            return wrapper?.History ?? [];
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch price history for market {ConditionId}", conditionId);
            return [];
        }
    }

    public async Task<OrderBook> GetOrderBookAsync(
        string tokenId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"/book?token_id={Uri.EscapeDataString(tokenId)}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<OrderBook>(JsonOptions, cancellationToken)
                ?? new OrderBook();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch order book for token {TokenId}", tokenId);
            return new OrderBook();
        }
    }

    private sealed class PriceHistoryWrapper
    {
        [JsonPropertyName("history")]
        public List<PricePoint>? History { get; set; }
    }
}

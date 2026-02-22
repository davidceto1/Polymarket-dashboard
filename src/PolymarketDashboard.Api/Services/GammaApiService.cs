using System.Text.Json;
using PolymarketDashboard.Core.Interfaces;
using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Api.Services;

public sealed class GammaApiService : IGammaApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<GammaApiService> _logger;

    public GammaApiService(HttpClient httpClient, ILogger<GammaApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Market>> GetActiveMarketsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                "/markets?closed=false&limit=50",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var markets = await response.Content
                .ReadFromJsonAsync<List<Market>>(JsonOptions, cancellationToken);

            return markets ?? [];
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch markets from Gamma API");
            return [];
        }
    }

    public async Task<Market?> GetMarketByConditionIdAsync(
        string conditionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/markets?conditionId={Uri.EscapeDataString(conditionId)}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var markets = await response.Content
                .ReadFromJsonAsync<List<Market>>(JsonOptions, cancellationToken);

            // Gamma ignores the conditionId filter and returns all markets, so we must
            // verify the returned market actually matches the requested conditionId.
            return markets?.FirstOrDefault(m =>
                string.Equals(m.ConditionId, conditionId, StringComparison.OrdinalIgnoreCase));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch market {ConditionId} from Gamma API", conditionId);
            return null;
        }
    }

    public async Task<Market?> GetMarketBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"/markets?slug={Uri.EscapeDataString(slug)}",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var markets = await response.Content
                .ReadFromJsonAsync<List<Market>>(JsonOptions, cancellationToken);

            return markets?.FirstOrDefault();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch market by slug {Slug} from Gamma API", slug);
            return null;
        }
    }
}

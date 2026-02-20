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
}

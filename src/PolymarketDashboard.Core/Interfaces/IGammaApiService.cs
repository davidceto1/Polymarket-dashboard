using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Core.Interfaces;

public interface IGammaApiService
{
    Task<IReadOnlyList<Market>> GetActiveMarketsAsync(CancellationToken cancellationToken = default);
    Task<Market?> GetMarketByConditionIdAsync(string conditionId, CancellationToken cancellationToken = default);
    Task<Market?> GetMarketBySlugAsync(string slug, CancellationToken cancellationToken = default);
}

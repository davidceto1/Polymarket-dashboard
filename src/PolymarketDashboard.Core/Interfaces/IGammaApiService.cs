using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Core.Interfaces;

public interface IGammaApiService
{
    Task<IReadOnlyList<Market>> GetActiveMarketsAsync(CancellationToken cancellationToken = default);
}

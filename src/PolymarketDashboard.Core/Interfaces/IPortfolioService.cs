using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Core.Interfaces;

public interface IPortfolioService
{
    Task<IReadOnlyList<Position>> GetPositionsAsync(string walletAddress, CancellationToken cancellationToken = default);
}

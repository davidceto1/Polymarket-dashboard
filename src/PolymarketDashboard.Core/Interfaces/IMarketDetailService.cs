using PolymarketDashboard.Core.Models;

namespace PolymarketDashboard.Core.Interfaces;

public interface IMarketDetailService
{
    Task<IReadOnlyList<PricePoint>> GetPriceHistoryAsync(string tokenId, CancellationToken cancellationToken = default);
    Task<OrderBook> GetOrderBookAsync(string tokenId, CancellationToken cancellationToken = default);
}

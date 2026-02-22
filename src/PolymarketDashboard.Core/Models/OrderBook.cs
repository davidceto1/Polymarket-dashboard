using System.Text.Json.Serialization;

namespace PolymarketDashboard.Core.Models;

public sealed class OrderBookEntry
{
    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("size")]
    public string Size { get; set; } = string.Empty;
}

public sealed class OrderBook
{
    [JsonPropertyName("bids")]
    public List<OrderBookEntry> Bids { get; set; } = [];

    [JsonPropertyName("asks")]
    public List<OrderBookEntry> Asks { get; set; } = [];
}

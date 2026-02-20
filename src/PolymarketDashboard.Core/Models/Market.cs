using System.Text.Json.Serialization;
using PolymarketDashboard.Core.Converters;

namespace PolymarketDashboard.Core.Models;

public sealed class Market
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    [JsonPropertyName("volume")]
    public string? Volume { get; set; }

    [JsonPropertyName("liquidity")]
    public string? Liquidity { get; set; }

    [JsonPropertyName("outcomePrices")]
    [JsonConverter(typeof(FlexibleStringArrayConverter))]
    public string[]? OutcomePrices { get; set; }

    [JsonPropertyName("outcomes")]
    [JsonConverter(typeof(FlexibleStringArrayConverter))]
    public string[]? Outcomes { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("closed")]
    public bool Closed { get; set; }
}

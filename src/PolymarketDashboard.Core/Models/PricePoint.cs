using System.Text.Json.Serialization;

namespace PolymarketDashboard.Core.Models;

public sealed class PricePoint
{
    /// <summary>Unix timestamp (seconds).</summary>
    [JsonPropertyName("t")]
    public long T { get; set; }

    /// <summary>Price 0–1.</summary>
    [JsonPropertyName("p")]
    public double P { get; set; }
}

using System.Text.Json.Serialization;
using PolymarketDashboard.Core.Converters;

namespace PolymarketDashboard.Core.Models;

public sealed class Market
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("conditionId")]
    public string ConditionId { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    [JsonPropertyName("startDate")]
    public string? StartDate { get; set; }

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

    [JsonPropertyName("clobTokenIds")]
    [JsonConverter(typeof(FlexibleStringArrayConverter))]
    public string[]? ClobTokenIds { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("closed")]
    public bool Closed { get; set; }

    // ── Order book snapshot ────────────────────────────────────────────────
    [JsonPropertyName("bestBid")]
    public double? BestBid { get; set; }

    [JsonPropertyName("bestAsk")]
    public double? BestAsk { get; set; }

    [JsonPropertyName("spread")]
    public double? Spread { get; set; }

    [JsonPropertyName("lastTradePrice")]
    public double? LastTradePrice { get; set; }

    // ── Price changes ──────────────────────────────────────────────────────
    [JsonPropertyName("oneHourPriceChange")]
    public double? OneHourPriceChange { get; set; }

    [JsonPropertyName("oneDayPriceChange")]
    public double? OneDayPriceChange { get; set; }

    [JsonPropertyName("oneWeekPriceChange")]
    public double? OneWeekPriceChange { get; set; }

    [JsonPropertyName("oneMonthPriceChange")]
    public double? OneMonthPriceChange { get; set; }

    [JsonPropertyName("oneYearPriceChange")]
    public double? OneYearPriceChange { get; set; }

    // ── Volume breakdown ───────────────────────────────────────────────────
    [JsonPropertyName("volume24hr")]
    public double? Volume24hr { get; set; }

    [JsonPropertyName("volume1wk")]
    public double? Volume1wk { get; set; }

    [JsonPropertyName("volume1mo")]
    public double? Volume1mo { get; set; }

    [JsonPropertyName("volume1yr")]
    public double? Volume1yr { get; set; }
}

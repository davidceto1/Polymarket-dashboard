namespace PolymarketDashboard.Core.Models;

public sealed class Position
{
    public string Asset        { get; set; } = string.Empty;
    public string ConditionId  { get; set; } = string.Empty;
    public string Title        { get; set; } = string.Empty;
    public string Slug         { get; set; } = string.Empty;
    public string Icon         { get; set; } = string.Empty;
    public string Outcome      { get; set; } = string.Empty;
    public string EndDate      { get; set; } = string.Empty;

    public double Size             { get; set; }
    public double AvgPrice         { get; set; }
    public double CurPrice         { get; set; }
    public double CurrentValue     { get; set; }
    public double CashPnl          { get; set; }
    public double PercentPnl       { get; set; }
    public double RealizedPnl      { get; set; }
    public double InitialValue     { get; set; }
    public double TotalBought      { get; set; }

    public bool Redeemable { get; set; }
    public bool NegativeRisk { get; set; }
}

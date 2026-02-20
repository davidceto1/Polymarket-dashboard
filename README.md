# Polymarket Dashboard

A lightweight ASP.NET Core (.NET 8) web application that displays active prediction markets from [Polymarket](https://polymarket.com) using their public [Gamma API](https://gamma-api.polymarket.com).

---

## Project structure

```
PolymarketDashboard.sln
└── src/
    ├── PolymarketDashboard.Core/          # Models & interfaces (no framework deps)
    │   ├── Converters/
    │   │   └── FlexibleStringArrayConverter.cs
    │   ├── Interfaces/
    │   │   └── IGammaApiService.cs
    │   └── Models/
    │       └── Market.cs
    └── PolymarketDashboard.Api/           # ASP.NET Core Web API
        ├── Controllers/
        │   └── MarketsController.cs       # GET /api/markets
        ├── Services/
        │   ├── GammaApiService.cs         # Typed HttpClient for Gamma API
        │   ├── MarketPollingService.cs    # Background cache-refresh service
        │   └── CacheKeys.cs
        ├── wwwroot/
        │   └── index.html                 # Single-file vanilla JS frontend
        └── Program.cs
```

---

## Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0 or later |

Verify with:

```bash
dotnet --version
```

---

## Running the project

```bash
# 1. Clone the repository (if you haven't already)
git clone <repo-url>
cd Polymarket-dashboard

# 2. Restore & run
dotnet run --project src/PolymarketDashboard.Api/PolymarketDashboard.Api.csproj
```

The application will start and print its listening URL (typically `http://localhost:5000`).
Open that URL in your browser — the dashboard loads immediately.

### With hot reload (development)

```bash
dotnet watch --project src/PolymarketDashboard.Api/PolymarketDashboard.Api.csproj
```

---

## API

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/markets` | Returns up to 50 active markets as JSON. Results are cached for 30 seconds. |

Example response item:

```json
{
  "id": "0x...",
  "question": "Will X happen by Y?",
  "endDate": "2025-12-31T00:00:00Z",
  "volume": "1234567.89",
  "liquidity": "98765.43",
  "outcomes": ["Yes", "No"],
  "outcomePrices": ["0.72", "0.28"],
  "active": true,
  "closed": false
}
```

---

## Architecture notes

- **PolymarketDashboard.Core** — zero framework dependencies; contains the `Market` model, the `IGammaApiService` interface, and a `FlexibleStringArrayConverter` (handles Polymarket's fields that may arrive as either a JSON array or a JSON-encoded string).
- **PolymarketDashboard.Api** — hosts the typed `HttpClient`, a `BackgroundService` that pre-warms and periodically refreshes `IMemoryCache`, and the `MarketsController`.
- **CORS** — any `localhost` origin is allowed; tighten in production.
- **Static files** — `wwwroot/index.html` is served at `/`; all unmatched routes fall back to it.

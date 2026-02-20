using PolymarketDashboard.Api.Services;
using PolymarketDashboard.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

builder.Services.AddMemoryCache();

// CORS – allow any localhost origin for development convenience
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.SetIsOriginAllowed(origin =>
                new Uri(origin).Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Typed HttpClient for the Gamma API
builder.Services.AddHttpClient<IGammaApiService, GammaApiService>(client =>
{
    client.BaseAddress = new Uri("https://gamma-api.polymarket.com");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// Background polling service – pre-warms and periodically refreshes the cache
builder.Services.AddHostedService<MarketPollingService>();

// ── Pipeline ──────────────────────────────────────────────────────────────────

var app = builder.Build();

app.UseCors();
app.UseStaticFiles();   // serves wwwroot/index.html at "/"
app.UseRouting();
app.MapControllers();

// Fallback: any non-API route returns the SPA shell
app.MapFallbackToFile("index.html");

app.Run();

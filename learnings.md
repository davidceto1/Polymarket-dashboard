# Learnings — Wrong Assumptions Corrected

## 1. Gamma API `?conditionId=` filter is silently ignored

**Assumption:** `GET /markets?conditionId={id}` on `gamma-api.polymarket.com` would return the single market matching that condition ID.

**Reality:** The Gamma API ignores the `conditionId` query parameter entirely and returns its default market list (first result is always the old Biden/Coronavirus market). Calling `FirstOrDefault()` on that list always returned Biden regardless of which market was requested.

**Fix:** Use `?slug=` instead — the Gamma API does honour the slug filter and returns exactly one matching market. Added `GetMarketBySlugAsync` and made the controller prefer slug lookup over conditionId lookup. Also added a conditionId equality check in `GetMarketByConditionIdAsync` so it returns `null` rather than a wrong market when the filter is ignored.

---

## 2. CLOB API hostname — `clob-api.polymarket.com` vs `clob.polymarket.com`

**Assumption:** The CLOB API (price history + order book) is hosted at `clob-api.polymarket.com`.

**Reality:** `clob-api.polymarket.com` fails DNS resolution in the WSL environment (`Could not resolve host`). The correct reachable hostname is `clob.polymarket.com` (no `-api` segment), which resolves and responds normally.

**Fix:** Changed the `HttpClient` base address in `Program.cs` from `https://clob-api.polymarket.com` to `https://clob.polymarket.com`.

---

## 3. Price history endpoint requires CLOB token ID, not `conditionId`

**Assumption:** The CLOB `/prices-history` endpoint identifies a market via its `conditionId` (the `0x...` hex string), passed as `?market={conditionId}`.

**Reality:** Passing the `conditionId` returns `{"history":[]}` — an empty result with HTTP 200. The endpoint actually requires the **CLOB token ID**: the long decimal numeric string found in the market's `clobTokenIds` array (index 0 = the Yes outcome token). Using the token ID returns the full price history correctly.

**Fix:** Renamed the `conditionId` parameter to `tokenId` throughout the stack (`IMarketDetailService`, `MarketDetailService`, `MarketDetailController`). Updated the frontend `openDetail` function to pass `yesTokenId` (i.e. `clobTokenIds[0]`) to `loadChart` instead of `conditionId`, and updated `loadChart` to send `?tokenId=` to the API.

---

## 4. Portfolio `conditionId` mismatch was actually a cache miss, not an ID format difference

**Assumption:** The `conditionId` returned by `data-api.polymarket.com` (portfolio positions) was in a different format than the `conditionId` in Gamma API market objects, explaining why `allMarkets.find(x => x.conditionId === conditionId)` always failed.

**Reality:** The two `conditionId` values are actually the same format and do match. The lookup was failing simply because portfolio position markets were not present in the 50-market active cache fetched by the polling service. Bug #1 (the ignored `?conditionId=` Gamma filter) was masking this, making it look like an identifier mismatch when the real issue was just a cache miss combined with a broken fallback.

**Fix:** No identifier conversion needed. Switching the backend fallback to slug-based lookup (the fix for bug #1) was sufficient to retrieve the correct market on a cache miss.

---

## 5. Background shell wrapper exiting does not mean the server stopped

**Assumption:** When a background Bash task reported its wrapper process as completed/exited, it was interpreted as the `dotnet run` server process having shut down.

**Reality:** The shell wrapper process and the `dotnet` child process have independent lifetimes. The wrapper can exit while the server keeps running, continuing to serve HTTP 200 responses on port 5000.

**Fix:** No code change. A `curl` health-check confirmed the server was still alive. The lesson is to verify actual server state with a request rather than inferring it from the wrapper process status.

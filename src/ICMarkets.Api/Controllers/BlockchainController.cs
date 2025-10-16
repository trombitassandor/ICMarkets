using System.Text.Json;
using ICMarkets.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ICMarkets.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockchainController : ControllerBase
{
    private readonly BlockchainService _service;

    public BlockchainController(BlockchainService service)
    {
        _service = service;
    }

    /// <summary>
    /// Fetch latest data from BlockCypher for a chain and store snapshot.
    /// </summary>
    [HttpPost("fetch/{chain}")]
    public async Task<IActionResult> Fetch(string chain, CancellationToken ct)
    {
        var ok = await _service.FetchAndStoreAsync(chain, ct);
        if (!ok) return BadRequest(new { error = "Unknown chain or failed to fetch" });
        return Ok(new { result = "stored" });
    }

    /// <summary>
    /// Get stored snapshots history for a chain (descending CreatedAt)
    /// </summary>
    [HttpGet("history/{chain}")]
    public async Task<IActionResult> History(string chain, int limit = 50, CancellationToken ct = default)
    {
        var items = await _service.GetHistoryAsync(chain, limit, ct);
        return Ok(items.Select(x => new { x.Id, x.Chain, x.CreatedAt, ApiResponseJson = JsonDocument.Parse(x.ApiResponseJson) }));
    }
}

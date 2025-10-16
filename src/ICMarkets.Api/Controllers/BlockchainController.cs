using System.Text.Json;
using ICMarkets.Api.Application;
using ICMarkets.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ICMarkets.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockchainController : ControllerBase
{
    private readonly Service _service;

    public BlockchainController(Service service)
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

        return ok 
            ? Ok(new { result = "stored" }) 
            : BadRequest(new { error = "Unknown chain or failed to fetch" });
    }

    /// <summary>
    /// Get stored snapshots history for a chain (descending CreatedAt)
    /// </summary>
    [HttpGet("history/{chain}")]
    public async Task<IActionResult> History(string chain, int limit = 50, CancellationToken ct = default)
    {
        var items = await _service.GetHistoryAsync(chain, limit, ct);

        var okValue = items.Select(blockchainSnapshot => new
        {
            blockchainSnapshot.Id,
            blockchainSnapshot.Chain,
            blockchainSnapshot.CreatedAt,
            ApiResponseJson = JsonDocument.Parse(
                blockchainSnapshot.ApiResponseJson)
        });
        
        var isOk = Ok(okValue);
        
        return isOk;
    }
}

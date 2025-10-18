using System.Text.Json;
using ICMarkets.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ICMarkets.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlockchainController(IMediator mediator) : ControllerBase
{
    //private readonly Service _service;

    /// <summary>
    /// Fetch latest data from BlockCypher for a chain and store snapshot.
    /// </summary>
    [HttpPost("fetch/{chain}")]
    public async Task<IActionResult> Fetch(string chain, CancellationToken ct)
    {
        var command = new FetchSnapshotCommand(chain);
        var ok = await mediator.Send(command, ct);

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
        var command = new GetHistoryQuery(chain, limit);
        var items = await mediator.Send(command, ct);

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
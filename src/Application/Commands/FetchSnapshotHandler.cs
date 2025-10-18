using Domain;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Commands;

public class FetchSnapshotHandler(
    IHttpClientFactory factory, 
    IUnitOfWork unitOfWork, 
    IConfiguration config)
    : IRequestHandler<FetchSnapshotCommand, bool>
{
    public async Task<bool> Handle(FetchSnapshotCommand command, CancellationToken ct)
    {
        var url = ResolveUrl(command.Chain);

        if (url is null)
        {
            return false;
        }

        var client = factory.CreateClient("blockcypher");
        var resp = await client.GetAsync(url, ct);

        if (!resp.IsSuccessStatusCode)
        {
            return false;
        }

        var json = await resp.Content.ReadAsStringAsync(ct);
        var blockchainSnapshot = new BlockchainSnapshot
        {
            Chain = command.Chain,
            ApiResponseJson = json,
            CreatedAt = DateTime.UtcNow
        };
        
        await unitOfWork.Repository.AddAsync(blockchainSnapshot, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
    
    private string? ResolveUrl(string chain)
    {
        var chainLower = chain.ToLowerInvariant();
        
        return chainLower switch
        {
            "eth" or "dash" or "btc" or "btc_test3" or "ltc" =>
                config[$"BlockCypher:{chainLower}"],
            _ => null
        };
    }
}
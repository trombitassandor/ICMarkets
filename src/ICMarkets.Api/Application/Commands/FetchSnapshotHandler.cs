using ICMarkets.Api.Domain;
using MediatR;

namespace ICMarkets.Api.Application.Commands;

public class FetchSnapshotHandler : IRequestHandler<FetchSnapshotCommand, bool>
{
    private readonly IHttpClientFactory _factory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public FetchSnapshotHandler(IHttpClientFactory factory, IUnitOfWork unitOfWork, IConfiguration config)
    {
        _factory = factory;
        _unitOfWork = unitOfWork;
        _config = config;
    }

    public async Task<bool> Handle(FetchSnapshotCommand command, CancellationToken ct)
    {
        var url = ResolveUrl(command.Chain);

        if (url is null)
        {
            return false;
        }

        var client = _factory.CreateClient("blockcypher");
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
        
        await _unitOfWork.Repository.AddAsync(blockchainSnapshot, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
    
    private string? ResolveUrl(string chain)
    {
        var chainLower = chain.ToLowerInvariant();
        
        return chainLower switch
        {
            "eth" or "dash" or "btc" or "btc_test3" or "ltc" =>
                _config[$"BlockCypher:{chainLower}"],
            _ => null
        };
    }
}
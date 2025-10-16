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
        var chain = command.Chain;
        var url = ResolveUrl(chain);

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
            Chain = chain,
            ApiResponseJson = json,
            CreatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.Repository.AddAsync(blockchainSnapshot, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return true;
    }
    
    private string? ResolveUrl(string chain)
    {
        return chain.ToLower() switch
        {
            "eth" => _config.GetSection("BlockCypher:eth").Value,
            "dash" => _config.GetSection("BlockCypher:dash").Value,
            "btc" => _config.GetSection("BlockCypher:btc").Value,
            "btc_test3" => _config.GetSection("BlockCypher:btc_test3").Value,
            "ltc" => _config.GetSection("BlockCypher:ltc").Value,
            _ => null
        };
    }
}
using ICMarkets.Api.Domain;

namespace ICMarkets.Api.Application;

public class BlockchainService : IBlockchainService
{
    private readonly IHttpClientFactory _factory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public BlockchainService(IHttpClientFactory factory,
        IUnitOfWork unitOfWork, IConfiguration config)
    {
        _factory = factory;
        _unitOfWork = unitOfWork;
        _config = config;
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

    public async Task<bool> FetchAndStoreAsync(string chain, CancellationToken ct = default)
    {
        var url = ResolveUrl(chain);
        
        if (url is null) return false;

        var client = _factory.CreateClient("blockcypher");
        var resp = await client.GetAsync(url, ct);
        
        if (!resp.IsSuccessStatusCode) return false;

        var json = await resp.Content.ReadAsStringAsync(ct);
        var snap = new BlockchainSnapshot
        {
            Chain = chain,
            ApiResponseJson = json,
            CreatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.Repository.AddAsync(snap, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<List<BlockchainSnapshot>> GetHistoryAsync(
        string chain, int limit = 100, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository.GetHistoryAsync(chain, limit, ct);
    }
}
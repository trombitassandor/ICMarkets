using ICMarkets.Api.Domain;

namespace ICMarkets.Api.Infrastructure;

public class BlockchainService
{
    private readonly IHttpClientFactory _factory;
    private readonly IBlockchainSnapshotRepository _repository; // New Dependency
    private readonly IConfiguration _config;

    public BlockchainService(IHttpClientFactory factory, IBlockchainSnapshotRepository repository, IConfiguration config)
    {
        _factory = factory;
        _repository = repository;
        _config = config;
    }

    // ... ResolveUrl method remains the same ...
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
        
        // Data access is delegated to the repository
        await _repository.AddAsync(snap, ct);
        await _repository.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<List<BlockchainSnapshot>> GetHistoryAsync(string chain, int limit = 100, CancellationToken ct = default)
    {
        // Data access is delegated to the repository
        return await _repository.GetHistoryAsync(chain, limit, ct);
    }
}
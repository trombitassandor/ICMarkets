using Domain;
using Microsoft.Extensions.Configuration;

namespace Application.ObsoleteService;

public class Service(
    IHttpClientFactory factory,
    IUnitOfWork unitOfWork,
    IConfiguration config)
    : IService
{
    public async Task<bool> FetchAndStoreAsync(string chain, CancellationToken ct = default)
    {
        var url = ResolveUrl(chain);

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
            Chain = chain,
            ApiResponseJson = json,
            CreatedAt = DateTime.UtcNow
        };
        
        await unitOfWork.Repository.AddAsync(blockchainSnapshot, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<List<BlockchainSnapshot>> GetHistoryAsync(
        string chain, int limit = 100, CancellationToken ct = default)
    {
        return await unitOfWork.Repository.GetHistoryAsync(chain, limit, ct);
    }

    private string? ResolveUrl(string chain)
    {
        return chain.ToLower() switch
        {
            "eth" => config.GetSection("BlockCypher:eth").Value,
            "dash" => config.GetSection("BlockCypher:dash").Value,
            "btc" => config.GetSection("BlockCypher:btc").Value,
            "btc_test3" => config.GetSection("BlockCypher:btc_test3").Value,
            "ltc" => config.GetSection("BlockCypher:ltc").Value,
            _ => null
        };
    }
}
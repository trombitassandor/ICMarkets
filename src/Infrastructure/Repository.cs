using ICMarkets.Domain;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Infrastructure;

public class Repository(AppDbContext db) : IRepository
{
    public async Task AddAsync(BlockchainSnapshot snapshot, CancellationToken ct = default)
    {
        await db.BlockchainSnapshots.AddAsync(snapshot, ct);
    }

    public async Task<List<BlockchainSnapshot>> GetHistoryAsync(
        string chain, int limit = 100, CancellationToken ct = default)
    {
        return await db.BlockchainSnapshots
            .Where(x => x.Chain == chain)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }
}
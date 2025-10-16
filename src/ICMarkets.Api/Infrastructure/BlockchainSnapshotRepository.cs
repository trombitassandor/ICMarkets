using ICMarkets.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Api.Infrastructure;

public class BlockchainSnapshotRepository : IBlockchainSnapshotRepository
{
    private readonly AppDbContext _db;

    public BlockchainSnapshotRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(BlockchainSnapshot snapshot, CancellationToken ct = default)
    {
        await _db.BlockchainSnapshots.AddAsync(snapshot, ct);
    }

    public async Task<List<BlockchainSnapshot>> GetHistoryAsync(
        string chain, int limit = 100, CancellationToken ct = default)
    {
        return await _db.BlockchainSnapshots
            .Where(x => x.Chain == chain)
            .OrderByDescending(x => x.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _db.SaveChangesAsync(ct);
    }
}
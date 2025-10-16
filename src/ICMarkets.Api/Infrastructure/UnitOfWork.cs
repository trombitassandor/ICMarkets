using ICMarkets.Api.Domain;

namespace ICMarkets.Api.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IBlockchainSnapshotRepository? _blockchainSnapshots;

    // Inject the DbContext to manage the transaction
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    public IBlockchainSnapshotRepository BlockchainSnapshots => 
        _blockchainSnapshots ??= new BlockchainSnapshotRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
// ICMarkets.Api.Infrastructure/UnitOfWork.cs

using ICMarkets.Api.Domain;
using ICMarkets.Api.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    // Private backing field for the repository
    private IBlockchainSnapshotRepository _blockchainSnapshots; 

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    // Lazy-load the repository implementation
    public IBlockchainSnapshotRepository BlockchainSnapshots => 
        _blockchainSnapshots ??= new BlockchainSnapshotRepository(_context); 

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    public void Dispose() => _context.Dispose();
}
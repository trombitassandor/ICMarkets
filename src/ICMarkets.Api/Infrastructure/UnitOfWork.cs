using ICMarkets.Api.Domain;

namespace ICMarkets.Api.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository? _blockchainSnapshots;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    public IRepository Repository => 
        _blockchainSnapshots ??= new Repository(_context);

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
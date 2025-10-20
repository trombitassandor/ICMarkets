using ICMarkets.Domain;

namespace ICMarkets.Infrastructure;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IRepository? _repository;

    public IRepository Repository => 
        _repository ??= new Repository(context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await context.SaveChangesAsync(ct);
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}
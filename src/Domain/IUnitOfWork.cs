namespace Domain;

public interface IUnitOfWork : IDisposable
{
    IRepository Repository { get; } 
        
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
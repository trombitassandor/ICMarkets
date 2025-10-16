// ICMarkets.Api.Domain/IUnitOfWork.cs

namespace ICMarkets.Api.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        // Add all repository interfaces here if needed, but the core is SaveChangesAsync
        IBlockchainSnapshotRepository BlockchainSnapshots { get; } 
        
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
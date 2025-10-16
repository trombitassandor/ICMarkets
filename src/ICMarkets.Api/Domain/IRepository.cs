// File: ICMarkets.Api.Domain/IBlockchainSnapshotRepository.cs

namespace ICMarkets.Api.Domain
{
    public interface IRepository
    {
        Task AddAsync(BlockchainSnapshot snapshot, CancellationToken ct = default);
        Task<List<BlockchainSnapshot>> GetHistoryAsync(string chain, int limit = 100, CancellationToken ct = default);
    }
}
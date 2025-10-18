using ICMarkets.Domain;

namespace ICMarkets.Application.ObsoleteService
{
    public interface IService
    {
        /// <summary>
        /// Fetches data for a specific blockchain, processes the response, and stores a snapshot in the database.
        /// </summary>
        /// <param name="chain">The blockchain identifier (e.g., "eth", "btc").</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if the fetch and store operation was successful, otherwise false.</returns>
        Task<bool> FetchAndStoreAsync(string chain, CancellationToken ct = default);

        /// <summary>
        /// Retrieves the history of stored snapshots for a specific blockchain, ordered descending by creation time.
        /// </summary>
        /// <param name="chain">The blockchain identifier.</param>
        /// <param name="limit">The maximum number of records to return.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of BlockchainSnapshot entities.</returns>
        Task<List<BlockchainSnapshot>> GetHistoryAsync(string chain, int limit = 100, CancellationToken ct = default);
    }
}
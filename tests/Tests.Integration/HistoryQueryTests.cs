using System.Text.Json;
using ICMarkets.Domain;
using ICMarkets.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ICMarkets.Tests.Integration;

public class HistoryQueryTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetHistory_SnapshotExists_ReturnsDataInDescendingOrder()
    {
        // ARRANGE: Chain to query
        const string chain = "ETH";
        var uri = $"/api/v1/history/{chain}"; 
        var expectedTimestamps = new List<DateTimeOffset>();
        
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 
            dbContext.BlockchainSnapshots.RemoveRange(dbContext.BlockchainSnapshots);
            await dbContext.SaveChangesAsync();
            
            var newestSnapshot = new BlockchainSnapshot { 
                Chain = chain, 
                CreatedAt = DateTime.Now.AddMinutes(3),
                ApiResponseJson = "{\"price\": 3000}"
            };
            
            var middleSnapshot = new BlockchainSnapshot { 
                Chain = chain, 
                CreatedAt = DateTime.Now.AddMinutes(2),
                ApiResponseJson = "{\"price\": 2000}"
            };

            var oldestSnapshot = new BlockchainSnapshot { 
                Chain = chain, 
                CreatedAt = DateTime.Now.AddMinutes(1),
                ApiResponseJson = "{\"price\": 1000}"
            };

            await dbContext.BlockchainSnapshots.AddRangeAsync(
                newestSnapshot, middleSnapshot, oldestSnapshot);
            await dbContext.SaveChangesAsync();
            
            // Store timestamps in the order we expect them back (Descending: T3, T2, T1)
            expectedTimestamps.Add(newestSnapshot.CreatedAt);
            expectedTimestamps.Add(middleSnapshot.CreatedAt);
            expectedTimestamps.Add(oldestSnapshot.CreatedAt);
        }

        // ACT: Hit the GET /history/{chain} endpoint
        var response = await _client.GetAsync(uri);
        
        // ASSERT 1: Verify success status code
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        
        // Assuming the API returns a list of BlockchainSnapshot objects (or a DTO mapping them)
        var actualSnapshots = JsonSerializer.Deserialize<List<BlockchainSnapshot>>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // ASSERT 2: Verify the correct number of items were returned
        Assert.NotNull(actualSnapshots);
        Assert.Equal(3, actualSnapshots!.Count);

        // ASSERT 3: Verify the data is returned in descending order (newest first)
        // Check if the timestamps are in descending order by comparing the actual sequence 
        // of timestamps with the expected descending sequence.
        var actualTimestamps = actualSnapshots
            .Select(blockchainSnapshot => blockchainSnapshot.CreatedAt)
            .ToList();
        
        Assert.Equivalent(expectedTimestamps, actualTimestamps);
    }
}
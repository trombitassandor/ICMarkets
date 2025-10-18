using Domain;
using Moq;
using Xunit;

namespace ICMarkets.Tests.Functional;

public class BlockchainControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public BlockchainControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        
        _factory.MockUow.Invocations.Clear();
        _factory.MockHttpClientFactory.Invocations.Clear();
    }

    [Theory]
    [InlineData("eth")]
    [InlineData("btc")]
    public async Task Fetch_ValidChain_ReturnsOkAndSavesToDatabase(string chain)
    {
        // Arrange
        var uri = $"/api/v1/blockchain/fetch?chain={chain}";

        // Act
        var response = await _client.PostAsync(uri, null);

        // Assert 1: Functional Check (HTTP Status)
        response.EnsureSuccessStatusCode(); // Checks for 2xx status codes
        
        // Assert 2: Functional Check (Response Content)
        var responseString = await response.Content.ReadAsStringAsync();
        // Assuming your controller returns "true" or a JSON success object
        Assert.Equal("true", responseString, ignoreCase: true); 

        // Assert 3: Integration Check (Database Mock Verification)
        _factory.MockUow.Verify(
            uow => uow.Repository.AddAsync(
                It.Is<BlockchainSnapshot>(s => s.Chain == chain), 
                It.IsAny<CancellationToken>()), 
            Times.Once,
            "The API handler must attempt to save a snapshot to the database.");
            
        _factory.MockUow.Verify(
            uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once,
            "The API handler must call SaveChangesAsync once to commit the transaction.");
            
        // Assert 4: Integration Check (External HTTP Client Verification)
        _factory.MockHttpClientFactory.Verify(
            factory => factory.CreateClient(It.IsAny<string>()),
            Times.Once,
            "The handler must create and use an HttpClient to fetch external data.");
    }
}
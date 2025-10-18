using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Application.Commands;
using Domain;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

namespace ICMarkets.Tests.Unit;

public class FetchSnapshotHandlerTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactory;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly IConfiguration _config;
    private readonly FetchSnapshotHandler _fetchSnapshotHandler;

    public FetchSnapshotHandlerTests()
    {
        _httpClientFactory = new Mock<IHttpClientFactory>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _config = BuildConfiguration();
        SetupHttpClientFactory();
        _fetchSnapshotHandler = new FetchSnapshotHandler(
            _httpClientFactory.Object, 
            _unitOfWork.Object,
            _config);
    }

    private void SetupHttpClientFactory()
    {
        var mockMessageHandler = new Mock<HttpMessageHandler>();
        mockMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"name\": \"test\", \"time\": \"2025-01-01T00:00:00Z\"}"),
            });


        var httpClient = new HttpClient(mockMessageHandler.Object);

        _httpClientFactory
            .Setup(httpClientFactory => 
                httpClientFactory.CreateClient(
                    It.IsAny<string>()))
            .Returns(httpClient);
    }

    private IConfiguration BuildConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string> {
            // Map the full colon-separated keys to the desired URLs
            {"BlockCypher:eth", "https://api.blockcypher.com/v1/eth/main"},
            {"BlockCypher:dash", "https://api.blockcypher.com/v1/dash/main"},
            {"BlockCypher:btc", "https://api.blockcypher.com/v1/btc/main"},
            {"BlockCypher:btc_test3", "https://api.blockcypher.com/v1/btc/test3"},
            {"BlockCypher:ltc", "https://api.blockcypher.com/v1/ltc/main"},
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesSnapshotAndReturnsTrue()
    {
        // Arrange
        var command = new FetchSnapshotCommand("eth");
        
        _unitOfWork
            .Setup(unitOfWork =>
                unitOfWork.Repository.AddAsync(
                    It.IsAny<BlockchainSnapshot>(),
                    It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _fetchSnapshotHandler.Handle(
            command, CancellationToken.None);

        // Assert
        Assert.True(result);
        
        _unitOfWork.Verify(unitOfWork => 
                unitOfWork.Repository.AddAsync(
                    It.IsAny<BlockchainSnapshot>(), 
                    It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _unitOfWork.Verify(unitOfWork =>
                unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
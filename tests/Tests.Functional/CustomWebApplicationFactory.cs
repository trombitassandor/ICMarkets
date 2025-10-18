using System.Net;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;

namespace ICMarkets.Tests.Functional;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUnitOfWork> MockUow { get; } = new Mock<IUnitOfWork>();
    public Mock<IHttpClientFactory> MockHttpClientFactory { get; } = new Mock<IHttpClientFactory>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // --- 1. MOCK IUnitOfWork (Database Layer) ---
            
            // a. Remove the real IUnitOfWork/DB Context service registration
            var uowDescriptor = services.SingleOrDefault(serviceDescriptor => 
                serviceDescriptor.ServiceType == typeof(IUnitOfWork));
            
            if (uowDescriptor != null)
            {
                services.Remove(uowDescriptor);
            }

            // b. Set up the MOCK IUnitOfWork's default behavior
            MockUow
                .Setup(uow => uow.Repository.AddAsync(
                    It.IsAny<BlockchainSnapshot>(), 
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            MockUow
                .Setup(uow => uow.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); // Return 1 row affected on save

            // c. Register the MOCKED instance
            services.AddSingleton(MockUow.Object);

            
            // --- 2. MOCK IHttpClientFactory (External HTTP Calls) ---
            
            // a. Remove the real IHttpClientFactory registration
            var httpFactoryDescriptor = services.SingleOrDefault(serviceDescriptor => 
                serviceDescriptor.ServiceType == typeof(IHttpClientFactory));
            
            if (httpFactoryDescriptor != null)
            {
                services.Remove(httpFactoryDescriptor);
            }

            // b. Set up a Mock HttpMessageHandler to return a fake success response
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    methodOrPropertyName: "SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    // Provide mock content that your API handler expects from the external service
                    Content = new StringContent("{ \"success\": true, \"data\": \"mocked_data\" }"),
                });

            // c. Set up the Mock IHttpClientFactory to return a client using the mock handler
            MockHttpClientFactory
                .Setup(httpClientFactory => 
                    httpClientFactory.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(mockHttpMessageHandler.Object));

            // d. Register the MOCKED instance
            services.AddSingleton(MockHttpClientFactory.Object);
        });
    }
}
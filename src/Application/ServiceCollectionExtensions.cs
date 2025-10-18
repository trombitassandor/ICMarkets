using ICMarkets.Application.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarkets.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(configurator => 
            configurator.RegisterServicesFromAssembly(
                typeof(FetchSnapshotCommand).Assembly));
        return services;
    }
}
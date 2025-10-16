using ICMarkets.Api.Application;
using ICMarkets.Api.Domain;

namespace ICMarkets.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("blockcypher");
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<Service>();
        return services;
    }
}

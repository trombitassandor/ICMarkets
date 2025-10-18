using Application.Commands;
using Application.ObsoleteService;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpClient("blockcypher");
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepository, Repository>();
        //services.AddScoped<Service>();
        services.AddMediatR(configurator => 
            configurator.RegisterServicesFromAssembly(
                typeof(FetchSnapshotCommand).Assembly));
        return services;
    }
}

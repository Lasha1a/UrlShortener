using UrlShortener.Application;
using UrlShortener.Infrastructure;
using UrlShortener.Persistence;

namespace UrlShortener.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddMainApiDi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationDi()
            .AddInfrastructureDi(configuration)
            .AddPersistenceDI(configuration);
        
        return services;
    }
}
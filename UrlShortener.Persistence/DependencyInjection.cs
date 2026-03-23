using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceDI(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
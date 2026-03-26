using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Interfaces.RedisCache;
using UrlShortener.Application.Interfaces.Url;
using UrlShortener.Infrastructure.BackgroundWorkers;
using UrlShortener.Infrastructure.Services.RedisCache;
using UrlShortener.Infrastructure.Services.Urls;

namespace UrlShortener.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
        });
        
        services.AddHostedService<ExpiredUrlCleanupService>();
        
        services.AddScoped<ICacheService, CacheService>();
        
        services.AddScoped<IBase62Service, Base62Service>();
        
        return services;
    }
}
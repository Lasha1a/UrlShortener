using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDi(this IServiceCollection services)
    {
        return services;
    }
}
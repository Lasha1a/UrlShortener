using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace UrlShortener.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDi(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
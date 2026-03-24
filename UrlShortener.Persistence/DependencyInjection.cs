using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Interfaces.Url;
using UrlShortener.Persistence.Data;
using UrlShortener.Persistence.Repositories.Urls;
using UrlShortener.Persistence.Settings;

namespace UrlShortener.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceDI(this IServiceCollection services, IConfiguration configuration)
    {
        //cassandra settings
        var cassandraSettings = configuration.GetSection("Cassandra").Get<CassandraSettings>();
        services.AddSingleton(cassandraSettings);
        
        //session factory - singleton cuz we need one connection for the app lifetime
        services.AddSingleton<CassandraSessionFactory>();
        
        //initializer
        services.AddSingleton<CassandraInitializer>();
        
        services.AddScoped<IUrlClickRepository, UrlClickRepository>();
        services.AddScoped<IUrlRepository, UrlRepository>();
        
        return services;
    }
}
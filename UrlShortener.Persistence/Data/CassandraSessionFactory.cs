using Cassandra;
using Microsoft.Extensions.Logging;
using UrlShortener.Persistence.Settings;

namespace UrlShortener.Persistence.Data;

public class CassandraSessionFactory : IDisposable
{
    private readonly ICluster _cluster;
    private readonly ISession _session;
    private readonly ILogger<CassandraSessionFactory> _logger;

    public CassandraSessionFactory(CassandraSettings settings, ILogger<CassandraSessionFactory> logger)
    {
        _logger = logger;

        _cluster = Cluster.Builder()
            .AddContactPoint(settings.ContactPoint)
            .WithPort(settings.Port)
            .Build();

        _session = _cluster.Connect();
        _logger.LogInformation("Cassandra connection established.");
    }
    
    public ISession GetSession() => _session;

    public void Dispose()
    {
        _session?.Dispose();
        _cluster?.Dispose();
    }
}
using Cassandra;
using Microsoft.Extensions.Logging;
using UrlShortener.Persistence.Settings;

namespace UrlShortener.Persistence.Data;

// creates the keyspace and tables similar as dotnet ef database update
public class CassandraInitializer
{
    private readonly ISession _session;
    private readonly CassandraSettings _settings;
    private readonly ILogger<CassandraInitializer> _logger;

    public CassandraInitializer(CassandraSessionFactory sessionFactory, CassandraSettings settings, ILogger<CassandraInitializer> logger)
    {
        _session = sessionFactory.GetSession();
        _settings = settings;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        await CreateKeyspaceAsync();
        await CreateTablesAsync();
        _logger.LogInformation("Cassandra keyspace and tables initialized.");
    }

    private Task CreateKeyspaceAsync()
    {
        var query = $@"
            CREATE KEYSPACE IF NOT EXISTS {_settings.Keyspace}
            WITH replication = {{
                'class': 'SimpleStrategy',
                'replication_factor': 1
            }};";

        _session.Execute(query);
        _session.Execute($"USE {_settings.Keyspace}");
        _logger.LogInformation("Keyspace '{Keyspace}' ready.", _settings.Keyspace);
        return Task.CompletedTask;
    }

    private Task CreateTablesAsync()
    {
        var urlsTable = $@"
            CREATE TABLE IF NOT EXISTS {_settings.Keyspace}.urls (
                short_code text PRIMARY KEY,
                original_url text,
                created_at timestamp,
                expires_at timestamp,
                click_count int,
                is_active boolean
            );";

        var clicksTable = $@"
            CREATE TABLE IF NOT EXISTS {_settings.Keyspace}.url_clicks (
                short_code text,
                clicked_at timestamp,
                user_agent text,
                ip_address text,
                PRIMARY KEY (short_code, clicked_at)
            ) WITH CLUSTERING ORDER BY (clicked_at DESC);";

        _session.Execute(urlsTable);
        _session.Execute(clicksTable);
        _logger.LogInformation("Tables 'urls' and 'url_clicks' ready.");
        return Task.CompletedTask;
    }
}
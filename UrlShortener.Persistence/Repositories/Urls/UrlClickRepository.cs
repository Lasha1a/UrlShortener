using Cassandra;
using UrlShortener.Application.Interfaces.Url;
using UrlShortener.Domain.Entities;
using UrlShortener.Persistence.Data;
using UrlShortener.Persistence.Settings;

namespace UrlShortener.Persistence.Repositories.Urls;

public class UrlClickRepository : IUrlClickRepository
{
    private readonly ISession _session;
    private readonly CassandraSettings _settings;

    public UrlClickRepository(CassandraSessionFactory sessionFactory, CassandraSettings settings)
    {
        _session = sessionFactory.GetSession();
        _settings = settings;
    }
    
    public Task RecordClickAsync(UrlClick click)
    {
        var query = $@"INSERT INTO {_settings.Keyspace}.url_clicks 
            (short_code, clicked_at, user_agent, ip_address) 
            VALUES (?, ?, ?, ?)";

        var statement = new SimpleStatement(
            query,
            click.ShortCode,
            click.ClickedAt,
            click.UserAgent,
            click.IpAddress);

        _session.Execute(statement);
        return Task.CompletedTask;
    }
    
    public Task<IEnumerable<UrlClick>> GetClicksByShortCodeAsync(string shortCode)
    {
        var query = $"SELECT * FROM {_settings.Keyspace}.url_clicks WHERE short_code = ?";
        var statement = new SimpleStatement(query, shortCode);
        var rows = _session.Execute(statement);

        var clicks = rows.Select(row => UrlClick.Restore(
            row.GetValue<string>("short_code"),
            row.GetValue<DateTimeOffset>("clicked_at").UtcDateTime,
            row.GetValue<string>("user_agent"),
            row.GetValue<string>("ip_address")));

        return Task.FromResult(clicks);
    }
}
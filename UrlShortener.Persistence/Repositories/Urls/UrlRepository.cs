using Cassandra;
using UrlShortener.Application.Interfaces.Url;
using UrlShortener.Domain.Entities;
using UrlShortener.Persistence.Data;
using UrlShortener.Persistence.Settings;

namespace UrlShortener.Persistence.Repositories.Urls;

public class UrlRepository : IUrlRepository 
{
    private readonly ISession _session;
    private readonly CassandraSettings _settings;

    public UrlRepository(CassandraSessionFactory sessionFactory, CassandraSettings settings)
    {
        _session = sessionFactory.GetSession();
        _settings = settings;
    }
    
    public Task<ShortenedUrl?> GetByShortCodeAsync(string shortCode)
    {
        var query = $"SELECT * FROM {_settings.Keyspace}.urls WHERE short_code = ?";
        var statement = new SimpleStatement(query, shortCode);
        var row = _session.Execute(statement).FirstOrDefault();

        if (row == null) return Task.FromResult<ShortenedUrl?>(null);

        var url = MapToEntity(row);
        return Task.FromResult<ShortenedUrl?>(url);
    }
    
    public Task CreateAsync(ShortenedUrl url)
    {
        if (url.ExpiresAt.HasValue)
        {
            var ttlSeconds = (int)(url.ExpiresAt.Value - DateTime.UtcNow).TotalSeconds;

            var query = $@"INSERT INTO {_settings.Keyspace}.urls 
            (short_code, original_url, created_at, expires_at, click_count, is_active) 
            VALUES (?, ?, ?, ?, ?, ?)
            USING TTL ?";

            var statement = new SimpleStatement(
                query,
                url.ShortCode,
                url.OriginalUrl,
                url.CreatedAt,
                url.ExpiresAt,
                url.ClickCount,
                url.IsActive,
                ttlSeconds);

            _session.Execute(statement);
        }
        else
        {
            var query = $@"INSERT INTO {_settings.Keyspace}.urls 
            (short_code, original_url, created_at, expires_at, click_count, is_active) 
            VALUES (?, ?, ?, ?, ?, ?)";

            var statement = new SimpleStatement(
                query,
                url.ShortCode,
                url.OriginalUrl,
                url.CreatedAt,
                url.ExpiresAt,
                url.ClickCount,
                url.IsActive);

            _session.Execute(statement);
        }

        return Task.CompletedTask;
    }
    
    public Task UpdateAsync(ShortenedUrl url)
    {
        var query = $@"UPDATE {_settings.Keyspace}.urls 
            SET original_url = ?, expires_at = ?, click_count = ?, is_active = ?
            WHERE short_code = ?";

        var statement = new SimpleStatement(
            query,
            url.OriginalUrl,
            url.ExpiresAt,
            url.ClickCount,
            url.IsActive,
            url.ShortCode);

        _session.Execute(statement);
        return Task.CompletedTask;
    }

    public Task<bool> ShortCodeExistAsync(string shortCode)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string shortCode)
    {
        var query = $"DELETE FROM {_settings.Keyspace}.urls WHERE short_code = ?";
        var statement = new SimpleStatement(query, shortCode);
        _session.Execute(statement);
        return Task.CompletedTask;
    }

    public Task<bool> ShortCodeExistsAsync(string shortCode)
    {
        var query = $"SELECT short_code FROM {_settings.Keyspace}.urls WHERE short_code = ?";
        var statement = new SimpleStatement(query, shortCode);
        var row = _session.Execute(statement).FirstOrDefault();
        return Task.FromResult(row != null);
    }

    public Task<IEnumerable<ShortenedUrl>> GetExpiredUrlsAsync()
    {
        var query = $"SELECT * FROM {_settings.Keyspace}.urls WHERE is_active = true AND expires_at > ? ALLOW FILTERING";
        var statement = new SimpleStatement(query, DateTime.UtcNow);
        var rows = _session.Execute(statement);

        var urls = rows.Select((row => MapToEntity(row)));
        return Task.FromResult(urls);
    }
    
    
    
    private static ShortenedUrl MapToEntity(Row row)
    {
        return ShortenedUrl.Restore(
            row.GetValue<string>("short_code"),
            row.GetValue<string>("original_url"),
            row.GetValue<DateTimeOffset>("created_at").UtcDateTime,
            row.GetValue<DateTimeOffset?>("expires_at")?.UtcDateTime,
            row.GetValue<int>("click_count"),
            row.GetValue<bool>("is_active"));
    }
}
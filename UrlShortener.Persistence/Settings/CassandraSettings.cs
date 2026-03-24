namespace UrlShortener.Persistence.Settings;

public class CassandraSettings
{
    public string ContactPoint { get; set; }
    public int Port { get; set; }
    public string Keyspace { get; set; }
}
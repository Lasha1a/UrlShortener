namespace UrlShortener.Domain.Entities;

public class UrlClick
{
    public string ShortCode { get; private set; }
    public DateTime ClickedAt { get; private set; }
    public string UserAgent { get; private set; }
    public string IpAddress { get; private set; }

    private UrlClick() { }

    public static UrlClick Create(string shortCode, string userAgent, string ipAddress)
    {
        return new UrlClick
        {
            ShortCode = shortCode,
            ClickedAt = DateTime.UtcNow,
            UserAgent = userAgent,
            IpAddress = ipAddress
        };
    }
}
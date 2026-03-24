namespace UrlShortener.Domain.Entities;

public class ShortenedUrl
{
    public Guid Id { get; private set; }
    public string ShortCode { get; private set; }
    public string OriginalUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public int ClickCount { get; private set; }
    public bool IsActive { get; private set; }
    
    private  ShortenedUrl()
    {}
    
    public static ShortenedUrl Create(string originalUrl, string shortCode, DateTime? expiresAt = null)
    {
        return new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            ShortCode = shortCode,
            OriginalUrl = originalUrl,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            ClickCount = 0,
            IsActive = true
        };
    }
    
    public static ShortenedUrl Restore(
        string shortCode,
        string originalUrl,
        DateTime createdAt,
        DateTime? expiresAt,
        int clickCount,
        bool isActive)
    {
        return new ShortenedUrl
        {
            ShortCode = shortCode,
            OriginalUrl = originalUrl,
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
            ClickCount = clickCount,
            IsActive = isActive
        };
    }

    public void UpdateUrl(string newOriginalUrl)
    {
        OriginalUrl = newOriginalUrl;
    }
    
    public void UpdateExpiration(DateTime? newExpiresAt)
    {
        ExpiresAt = newExpiresAt;
    }
    
    public void IncrementClickCount()
    {
        ClickCount++;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsExpired()
    {
        return ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    }
}
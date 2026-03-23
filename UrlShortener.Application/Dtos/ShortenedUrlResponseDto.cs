namespace UrlShortener.Application.Dtos;

public class ShortenedUrlResponseDto
{
    public string ShortCode { get; set; }
    public string ShortUrl { get; set; }
    public string OriginalUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int ClickCount { get; set; }
    public bool IsActive { get; set; }
}
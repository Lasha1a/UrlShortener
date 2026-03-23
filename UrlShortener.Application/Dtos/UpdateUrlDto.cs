namespace UrlShortener.Application.Dtos;

public class UpdateUrlDto
{
    public string? OriginalUrl { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
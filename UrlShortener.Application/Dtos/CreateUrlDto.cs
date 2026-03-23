namespace UrlShortener.Application.Dtos;

public class CreateUrlDto
{
    public string OriginalUrl { get; set; }
    public string? CustomAlias { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
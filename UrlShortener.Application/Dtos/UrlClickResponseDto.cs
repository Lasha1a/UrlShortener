namespace UrlShortener.Application.Dtos;

public class UrlClickResponseDto //response dto for analytics
{
    public string ShortCode { get; set; }
    public DateTime ClickedAt { get; set; }
    public string UserAgent { get; set; }
    public string IpAddress { get; set; }
}
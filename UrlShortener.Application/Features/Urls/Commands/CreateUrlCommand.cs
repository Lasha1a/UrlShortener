using MediatR;
using UrlShortener.Application.Dtos;
using UrlShortener.Application.Interfaces.RedisCache;
using UrlShortener.Application.Interfaces.Url;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Features.Urls.Commands;

public record CreateUrlCommand(string OriginalUrl,
    string? CustomAlias,
    DateTime? ExpiresAt) : IRequest<ShortenedUrlResponseDto>;

public class CreateUrlCommandHandler : IRequestHandler<CreateUrlCommand, ShortenedUrlResponseDto>
{
    private readonly IUrlRepository _urlRepository;
    private readonly ICacheService _cacheService;

    public CreateUrlCommandHandler(IUrlRepository urlRepository, ICacheService cacheService)
    {
        _urlRepository = urlRepository;
        _cacheService = cacheService;
    }

    public async Task<ShortenedUrlResponseDto> Handle(CreateUrlCommand request, CancellationToken cancellationToken)
    {
        var shortCode = request.CustomAlias ?? GenerateShortCode();

        var exists = await _urlRepository.ShortCodeExistAsync(shortCode);
        if (exists)
        {
            throw new InvalidOperationException($"Short code '{shortCode}' already exists.");
        }
        
        var shortenedUrl = ShortenedUrl.Create(request.OriginalUrl, shortCode, request.ExpiresAt);

        await _urlRepository.CreateAsync(shortenedUrl);
        
        var result = MapToDto(shortenedUrl);
        
        await _cacheService.SetAsync($"url:{shortCode}", result, TimeSpan.FromMinutes(30));
        
        return result;
    }
    
    private static string GenerateShortCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 7)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    private static ShortenedUrlResponseDto MapToDto(ShortenedUrl url)
    {
        return new ShortenedUrlResponseDto
        {
            ShortCode = url.ShortCode,
            ShortUrl = $"https://short.ly/{url.ShortCode}",
            OriginalUrl = url.OriginalUrl,
            CreatedAt = url.CreatedAt,
            ExpiresAt = url.ExpiresAt,
            ClickCount = url.ClickCount,
            IsActive = url.IsActive
        };
    }
}
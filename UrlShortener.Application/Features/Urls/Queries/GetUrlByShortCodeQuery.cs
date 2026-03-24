using MediatR;
using UrlShortener.Application.Dtos;
using UrlShortener.Application.Interfaces.RedisCache;
using UrlShortener.Application.Interfaces.Url;

namespace UrlShortener.Application.Features.Urls.Queries;

public record GetUrlByShortCodeQuery(string ShortCode) : IRequest<ShortenedUrlResponseDto>;

public class GetUrlByShortCodeQueryHandler : IRequestHandler<GetUrlByShortCodeQuery, ShortenedUrlResponseDto>
{
    private readonly IUrlRepository _urlRepository;
    private readonly ICacheService _cacheService;

    public GetUrlByShortCodeQueryHandler(IUrlRepository urlRepository, ICacheService cacheService)
    {
        _urlRepository = urlRepository;
        _cacheService = cacheService;
    }

    public async Task<ShortenedUrlResponseDto> Handle(GetUrlByShortCodeQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"url:{request.ShortCode}";
        
        var cached = await _cacheService.GetAsync<ShortenedUrlResponseDto>(cacheKey);
        if (cached != null) return cached;

        var url = await _urlRepository.GetByShortCodeAsync(request.ShortCode);
        if (url == null)
            throw new KeyNotFoundException($"Short code '{request.ShortCode}' not found.");

        if (url.IsExpired())
            throw new InvalidOperationException($"Short code '{request.ShortCode}' has expired.");

        var result = new ShortenedUrlResponseDto
        {
            ShortCode = url.ShortCode,
            ShortUrl = $"https://short.ly/{url.ShortCode}",
            OriginalUrl = url.OriginalUrl,
            CreatedAt = url.CreatedAt,
            ExpiresAt = url.ExpiresAt,
            ClickCount = url.ClickCount,
            IsActive = url.IsActive
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

        return result;
    }
}   
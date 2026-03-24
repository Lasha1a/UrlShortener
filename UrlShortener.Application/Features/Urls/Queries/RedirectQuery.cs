using MediatR;
using UrlShortener.Application.Interfaces.RedisCache;
using UrlShortener.Application.Interfaces.Url;

namespace UrlShortener.Application.Features.Urls.Queries;

public record RedirectQuery(string ShortCode) : IRequest<string>;

public class RedirectQueryHandler : IRequestHandler<RedirectQuery, string>
{
    private readonly IUrlRepository _urlRepository;
    private readonly ICacheService _cacheService;

    public RedirectQueryHandler(IUrlRepository urlRepository, ICacheService cacheService)
    {
        _urlRepository = urlRepository;
        _cacheService = cacheService;
    }

    public async Task<string> Handle(RedirectQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"url:{request.ShortCode}";

        var cached = await _cacheService.GetAsync<string>(cacheKey);
        if (cached != null) return cached;

        var url = await _urlRepository.GetByShortCodeAsync(request.ShortCode);
        if (url == null)
            throw new KeyNotFoundException($"Short code '{request.ShortCode}' not found.");

        if (url.IsExpired() || !url.IsActive)
            throw new InvalidOperationException($"Short code '{request.ShortCode}' is no longer active.");

        await _cacheService.SetAsync(cacheKey, url.OriginalUrl, TimeSpan.FromMinutes(30));

        return url.OriginalUrl;
    }
}
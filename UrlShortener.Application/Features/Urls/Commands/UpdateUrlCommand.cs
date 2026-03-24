using MediatR;
using UrlShortener.Application.Interfaces.RedisCache;
using UrlShortener.Application.Interfaces.Url;

namespace UrlShortener.Application.Features.Urls.Commands;

public record UpdateUrlCommand(
    string ShortCode,
    string? OriginalUrl,
    DateTime? ExpiresAt) :  IRequest<bool>;
    
public class UpdateUrlCommandHandler : IRequestHandler<UpdateUrlCommand, bool>
{
    private readonly IUrlRepository _urlRepository;
    private readonly ICacheService _cacheService;

    public UpdateUrlCommandHandler(IUrlRepository urlRepository, ICacheService cacheService)
    {
        _urlRepository = urlRepository;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(UpdateUrlCommand request, CancellationToken cancellationToken)
    {
        var url = await _urlRepository.GetByShortCodeAsync(request.ShortCode);
        if (url == null)
            throw new KeyNotFoundException($"Short code '{request.ShortCode}' not found.");

        if (request.OriginalUrl != null)
            url.UpdateUrl(request.OriginalUrl);

        if (request.ExpiresAt.HasValue)
            url.UpdateExpiration(request.ExpiresAt);

        await _urlRepository.UpdateAsync(url);

        // Invalidate cache
        await _cacheService.RemoveAsync($"url:{request.ShortCode}");

        return true;
    }
}
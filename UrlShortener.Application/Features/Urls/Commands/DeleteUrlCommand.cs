using MediatR;
using UrlShortener.Application.Interfaces.RedisCache;
using UrlShortener.Application.Interfaces.Url;

namespace UrlShortener.Application.Features.Urls.Commands;

public record DeleteUrlCommand(string ShortCode) : IRequest<bool>;

public class DeleteUrlCommandHandler : IRequestHandler<DeleteUrlCommand, bool>
{
    private readonly IUrlRepository _urlRepository;
    private readonly ICacheService _cacheService;

    public DeleteUrlCommandHandler(IUrlRepository urlRepository, ICacheService cacheService)
    {
        _urlRepository = urlRepository;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteUrlCommand request, CancellationToken cancellationToken)
    {
        var url = await _urlRepository.GetByShortCodeAsync(request.ShortCode);
        if (url == null)
            throw new KeyNotFoundException($"Short code '{request.ShortCode}' not found.");

        await _urlRepository.DeleteAsync(request.ShortCode);

        // Invalidate cache
        await _cacheService.RemoveAsync($"url:{request.ShortCode}");

        return true;
    }
}
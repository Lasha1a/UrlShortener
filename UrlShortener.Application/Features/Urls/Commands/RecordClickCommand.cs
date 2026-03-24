using MediatR;
using UrlShortener.Application.Interfaces.Url;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Features.Urls.Commands;

public record RecordClickCommand(
    string ShortCode,
    string UserAgent,
    string IpAddress) : IRequest;

public class RecordClickCommandHandler : IRequestHandler<RecordClickCommand>
{
    private readonly IUrlClickRepository _urlClickRepository;
    private readonly IUrlRepository _urlRepository;

    public RecordClickCommandHandler(
        IUrlClickRepository urlClickRepository,
        IUrlRepository urlRepository)
    {
        _urlClickRepository = urlClickRepository;
        _urlRepository = urlRepository;
    }

    public async Task Handle(RecordClickCommand request, CancellationToken cancellationToken)
    {
        var click = UrlClick.Create(request.ShortCode, request.UserAgent, request.IpAddress);
        await _urlClickRepository.RecordClickAsync(click);

        // Increment click count on the url
        var url = await _urlRepository.GetByShortCodeAsync(request.ShortCode);
        if (url != null)
        {
            url.IncrementClickCount();
            await _urlRepository.UpdateAsync(url);
        }
    }
}
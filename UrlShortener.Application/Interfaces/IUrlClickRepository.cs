using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces;

public interface IUrlClickRepository
{
    Task RecordClickAsync(UrlClick Url);
    Task<IEnumerable<UrlClick>> GetClicksByShortCodeAsync(string shortCode);
}
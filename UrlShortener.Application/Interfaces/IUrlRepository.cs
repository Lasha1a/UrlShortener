using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Interfaces;

public interface IUrlRepository
{
    Task<ShortenedUrl> GetByShortCodeAsync(string shortCode);
    Task CreateAsync(ShortenedUrl Url);
    Task DeleteAsync(string shortCode);
    Task UpdateAsync(ShortenedUrl Url);
    Task<bool> ShortCodeExistAsync(string shortCode);
}
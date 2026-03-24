namespace UrlShortener.Application.Interfaces.Url;

public interface IBase62Service
{
    string Encode(long Number);
    string GenerateShortCode();
}
using UrlShortener.Application.Interfaces.Url;

namespace UrlShortener.Infrastructure.Services.Urls;

public class Base62Service : IBase62Service
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int Base = 62;

    public string Encode(long number)
    {
        if (number == 0) return Characters[0].ToString();
        
        var result = new Stack<char>();
        while (number > 0)
        {
            result.Push(Characters[(int)(number % Base)]);
            number /= Base;
        }
        
        return new string(result.ToArray());
    }

    public string GenerateShortCode()
    {
        var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var code = Encode(timeStamp);
        
        // ensure minimum 6 characters
        return code.Length >= 6 ? code[..7] : code.PadLeft(6, 'a');
    }
}
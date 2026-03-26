using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UrlShortener.Application.Interfaces.Url;

namespace UrlShortener.Infrastructure.BackgroundWorkers;

public class ExpiredUrlCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExpiredUrlCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

    public ExpiredUrlCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<ExpiredUrlCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting expired url cleanup");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupExpiredUrlsAsync();
            await Task.Delay(_interval, stoppingToken);
        }
    }
    
    private async Task CleanupExpiredUrlsAsync()
    {
        try
        {
            _logger.LogInformation("Running expired URL cleanup at {Time}", DateTime.UtcNow);

            using var scope = _scopeFactory.CreateScope();
            var urlRepository = scope.ServiceProvider.GetRequiredService<IUrlRepository>();

            var expiredUrls = await urlRepository.GetExpiredUrlsAsync();

            foreach (var url in expiredUrls)
            {
                url.Deactivate();
                await urlRepository.UpdateAsync(url);
                _logger.LogInformation("Deactivated expired URL: {ShortCode}", url.ShortCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during expired URL cleanup.");
        }
    }
}
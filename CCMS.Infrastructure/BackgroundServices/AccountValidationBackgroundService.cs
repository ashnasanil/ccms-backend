using CCMS.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
namespace CCMS.Infrastructure.BackgroundServices;

public class AccountValidationBackgroundService
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AccountValidationBackgroundService(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope =
                _scopeFactory.CreateScope();

            var batchService =
                scope.ServiceProvider
                .GetRequiredService<IBatchService>();

            await batchService.RunAsync(false);

            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stoppingToken);
        }
    }
}
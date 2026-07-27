using API.Rentals;

namespace API.BackgroundServices;

public class RentalsWorker(ILogger<RentalsWorker> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    // 0 = Midnight
    // 12 = Noon
    private static readonly TimeSpan RunAlwaysAt = TimeSpan.FromHours(12);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextRun();
            
            logger.LogInformation($"Next job will run in {delay}");
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested)
                break;

            logger.LogInformation("Running job");

            await RunJob();
        }
    }

    private TimeSpan GetDelayUntilNextRun()
    {
        return TimeSpan.FromSeconds(15);
        /*
         var now = DateTime.Now;

        var nextRun = now.Date + RunAlwaysAt;

        if (now >= nextRun)
            nextRun = nextRun.AddDays(1);

        return nextRun - now;
        */
    }

    private async Task RunJob()
    {
        using var scope = scopeFactory.CreateScope();
        var rentalService = scope.ServiceProvider.GetRequiredService<RentalService>();
        await rentalService.ProcessOverDueRentals();
        
        await Task.CompletedTask;
    }
}
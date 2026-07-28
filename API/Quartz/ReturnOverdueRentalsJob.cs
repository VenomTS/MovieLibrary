using API.Rentals;
using Quartz;

namespace API.Quartz;

public class ReturnOverdueRentalsJob(ILogger<ReturnOverdueRentalsJob> logger, RentalService rentalService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation($"Job executed at {DateTime.Now}");

        await rentalService.ProcessOverDueRentals();
    }
}
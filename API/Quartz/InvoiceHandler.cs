using Quartz;

namespace API.Quartz;

public class InvoiceHandler : IJob
{
    /*
     * Steps:
     * 1. Mark all previously unsuccessful as "In Progress"
     * 2. Get all scheduled for today (that are not in InvoiceDelivery as 'In Progress')
     * 3. Mark all of them as "In Progress"
     * 4. Update Scheduler
     * 5. Start sending those "In Progress"
     */
    public Task Execute(IJobExecutionContext context)
    {
        throw new NotImplementedException();
    }

    private async Task MarkFailedInvoices()
    {
        await Task.CompletedTask;
    }

    private async Task MarkScheduledInvoices()
    {
        await Task.CompletedTask;
    }

    private async Task DeleteOldInvoiceDeliveries()
    {
        await Task.CompletedTask;
    }
}
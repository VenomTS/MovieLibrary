using API.InvoiceDeliveries;
using API.Invoices;
using API.Schedules;
using Quartz;

namespace API.Quartz;

public class SendInvoicesJob(ScheduleService scheduleService, 
    InvoiceDeliveryService invoiceDeliveryService, 
    InvoiceService invoiceService,
    InvoiceSendingService invoiceSendingService) : IJob
{
    
    /*
     * Steps:
     * 1. Mark all previously unsuccessful as "In Progress"
     * 2. Get all scheduled for today (that are not in InvoiceDelivery as 'In Progress')
     * 3. Mark all of them as "In Progress"
     * 4. Update Scheduler
     * 5. Start sending those "In Progress"
     */
    
    public async Task Execute(IJobExecutionContext context)
    {
        await SendUnsuccessfulInvoices();
        await SendSchedulesInvoices();
    }

    private async Task SendSchedulesInvoices()
    {
        var scheduled = await scheduleService.GetScheduledAsync();
        var scheduledIds = scheduled.Select(x => x.Id).ToList();

        var scheduledInvoices = await invoiceService.GetScheduledInvoices(scheduledIds);

        foreach (var invoice in scheduledInvoices)
            await invoiceSendingService.SendNewInvoiceAsync(invoice);
    }

    private async Task SendUnsuccessfulInvoices()
    {
        await invoiceDeliveryService.ResendUnsuccessfulInvoicesAsync();
    }
}
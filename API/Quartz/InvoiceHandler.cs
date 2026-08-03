using API.InvoiceDeliveries;
using API.Invoices;
using API.InvoiceTemplates;
using API.Schedules;
using Models.Schedules;
using Quartz;

namespace API.Quartz;

public class InvoiceHandler(InvoiceDeliveryService invoiceDeliveryService, 
    ScheduleService scheduleService,
    InvoiceService invoiceService,
    InvoiceTemplateService invoiceTemplateService) : IJob
{

    private const int RemoveInvoiceDeliveriesAfterMonths = 3;
    
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
        await MarkFailedInvoices();
        await MarkScheduledInvoices();
        await StartSendingInvoices();
        await DeleteOldInvoiceDeliveries();
    }

    private async Task MarkFailedInvoices()
    {
        // Mark all failed invoices as 'InProgress'
        await invoiceDeliveryService.QueueUnsuccessfulInvoicesAsync();
    }

    private async Task MarkScheduledInvoices()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var scheduledInvoices = await invoiceTemplateService.GetScheduledInvoicesAsync(today);
        var uniqueSchedules = new List<Schedule>();
        
        foreach (var scheduledInvoice in scheduledInvoices)
        {
            await invoiceService.CreateAutomaticInvoice(scheduledInvoice);
            
            if (uniqueSchedules.Any(x => x.Id == scheduledInvoice.ScheduleId))
                continue;

            uniqueSchedules.Add(scheduledInvoice.Schedule);
        }

        await UpdateSchedules(uniqueSchedules);
    }

    private async Task UpdateSchedules(List<Schedule> uniqueSchedules)
    {
        foreach (var uniqueSchedule in uniqueSchedules)
            await scheduleService.UpdateNextOccurrenceAsync(uniqueSchedule);
    }
    
    private async Task DeleteOldInvoiceDeliveries()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var deleteBefore = today.AddMonths(-RemoveInvoiceDeliveriesAfterMonths);
        await invoiceDeliveryService.DeleteInvoiceDeliveriesAsync(deleteBefore);
    }

    private async Task StartSendingInvoices()
    {
        await invoiceDeliveryService.SendInvoicesAsync();
    }
}
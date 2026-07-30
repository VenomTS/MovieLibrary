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
        Console.WriteLine("Running InvoiceHandler");
        await MarkFailedInvoices();
        await MarkScheduledInvoices();
        await StartSendingInvoices();
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
            await invoiceService.CreateInvoice(scheduledInvoice);
            
            // Crash Me Pls
            var random = new Random();
            var randomValue = random.NextDouble();
            Console.WriteLine(randomValue);
            if(randomValue < 0.25)
                Environment.FailFast("Simulated crash");
            
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
        await Task.CompletedTask;
    }

    private async Task StartSendingInvoices()
    {
        await invoiceDeliveryService.SendInvoicesAsync();
        // var invoices = await invoiceDeliveryService.GetScheduledInvoiceDeliveries();
        //
        // foreach (var invoice in invoices)
        //     await invoiceSendingService.SendInvoiceAsync(invoice);
    }
}
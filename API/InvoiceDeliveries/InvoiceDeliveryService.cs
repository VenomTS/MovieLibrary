using API.Invoices;
using Microsoft.EntityFrameworkCore;
using Models.InvoiceDeliveries;
using Repositories;

namespace API.InvoiceDeliveries;

public class InvoiceDeliveryService(IRepositoryManager repositoryManager, InvoiceSendingService invoiceSendingService)
{
    private async Task DeleteOldInvoiceDeliveriesAsync()
    {
        await repositoryManager.InvoiceDeliveries.AsQueryable()
            .Where(x => x.DateCreated < DateOnly.FromDateTime(DateTime.Now.AddMonths(-3)))
            .ExecuteDeleteAsync();
    }

    public async Task QueueUnsuccessfulInvoicesAsync()
    {
        await repositoryManager.InvoiceDeliveries.QueueUnsuccessfulInvoicesAsync();
    }

    public async Task<List<InvoiceDelivery>> GetScheduledInvoiceDeliveries()
    {
        return await repositoryManager.InvoiceDeliveries.GetInProgressInvoicesAsync();
    }
    
    /*
    
    private async Task<List<InvoiceDelivery>> GetUnsuccessfulAsync()
    {
        var unsuccessfulDeliveries = await repositoryManager.InvoiceDeliveries.AsQueryable()
            .Where(x => x.Status != InvoiceDeliveryStatus.Successful &&
                        !repositoryManager.InvoiceDeliveries.AsQueryable()
                            .Any(y => x.InvoiceId == y.InvoiceId &&
                                      y.Status == InvoiceDeliveryStatus.Successful &&
                                      x.OriginalDate == y.OriginalDate &&
                                      x.ScheduleId == y.ScheduleId))
            .Include(x => x.Invoice)
            .ToListAsync();
        return unsuccessfulDeliveries;
    }

    public async Task ResendUnsuccessfulInvoicesAsync()
    {
        var unsuccessful = await GetUnsuccessfulAsync();

        foreach (var attempt in unsuccessful)
            await invoiceSendingService.ResentInvoiceAsync(attempt);
    }
    */
}
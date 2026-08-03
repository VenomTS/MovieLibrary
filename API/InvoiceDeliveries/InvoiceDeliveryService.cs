using API.Invoices;
using Microsoft.EntityFrameworkCore;
using Models.InvoiceDeliveries;
using Repositories;

namespace API.InvoiceDeliveries;

public class InvoiceDeliveryService(IRepositoryManager repositoryManager, InvoiceSendingService invoiceSendingService)
{
    public async Task DeleteInvoiceDeliveriesAsync(DateOnly date)
    {
        await repositoryManager.InvoiceDeliveries.AsQueryable()
            .Where(x => x.DateCreated < date)
            .ExecuteDeleteAsync();
    }

    public async Task QueueUnsuccessfulInvoicesAsync()
    {
        await repositoryManager.InvoiceDeliveries.QueueUnsuccessfulInvoicesAsync();
    }

    private async Task<List<InvoiceDelivery>> GetScheduledInvoiceDeliveries()
    {
        return await repositoryManager.InvoiceDeliveries.GetInProgressInvoicesAsync();
    }
    
    public async Task SendInvoicesAsync()
    {
        var scheduledInvoiceDeliveries = await GetScheduledInvoiceDeliveries();
        foreach(var invoiceDelivery in scheduledInvoiceDeliveries)
            await invoiceSendingService.SendInvoiceAsync(invoiceDelivery);
    }
}
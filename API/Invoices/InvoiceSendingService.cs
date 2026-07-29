using Models;
using Models.InvoiceDeliveries;
using Repositories;

namespace API.Invoices;

public class InvoiceSendingService(IRepositoryManager repositoryManager)
{
    public async Task<bool> SendNewInvoiceAsync(Invoice invoice)
    {
        var invoiceDelivery = await MarkAsInProgressAsync(invoice, DateOnly.FromDateTime(DateTime.Now));
        
        // Simulate sending invoice
        // SendMail(invoice);
        
        // 25% chance of fail
        var random = new Random();
        if (random.NextDouble() < 0.99)
        {
            invoiceDelivery.Status = InvoiceDeliveryStatus.Failed;
            await repositoryManager.SaveChangesAsync();
            return false;
        }

        invoiceDelivery.Status = InvoiceDeliveryStatus.Successful;
        await repositoryManager.SaveChangesAsync();
        return true;
    }

    public async Task ResentInvoiceAsync(InvoiceDelivery attempt)
    {
        attempt = await MarkAsInProgressAsync(attempt);
        
        var invoice = attempt.Invoice;
        
        // Attempt send
        // SendMail(invoice)
        
        var random = new Random();
        if (random.NextDouble() < 0.25)
        {
            // Failed
            attempt.Status = InvoiceDeliveryStatus.Failed;
            await repositoryManager.SaveChangesAsync();
            return;
        }

        attempt.Status = InvoiceDeliveryStatus.Successful;
        await repositoryManager.SaveChangesAsync();
    }

    private async Task<InvoiceDelivery> MarkAsInProgressAsync(Invoice invoice, DateOnly originalDate)
    {
        var invoiceDelivery = new InvoiceDelivery
        {
            InvoiceId = invoice.Id,
            ScheduleId = invoice.ScheduleId,
            Status = InvoiceDeliveryStatus.InProgress,
            AttemptedAt = DateOnly.FromDateTime(DateTime.Now),
            OriginalDate = originalDate,
        };
        
        await repositoryManager.InvoiceDeliveries.CreateAsync(invoiceDelivery);
        await repositoryManager.SaveChangesAsync();

        return invoiceDelivery;
    }

    private async Task<InvoiceDelivery> MarkAsInProgressAsync(InvoiceDelivery invoiceDelivery)
    {
        invoiceDelivery.Id = Guid.NewGuid();
        invoiceDelivery.Status = InvoiceDeliveryStatus.InProgress;
        await repositoryManager.InvoiceDeliveries.CreateAsync(invoiceDelivery);
        await repositoryManager.SaveChangesAsync();
        return invoiceDelivery;
    }
}
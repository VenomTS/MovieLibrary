using API.InvoiceDeliveries;
using Models.InvoiceDeliveries;
using Repositories;

namespace API.Invoices;

public class InvoiceSendingService(IRepositoryManager repositoryManager)
{
    public async Task SendInvoiceAsync(InvoiceDelivery invoiceDelivery)
    {
        // Simulate sending an invoice
        // SendMail(invoiceDelivery.Invoice);
        var random = new Random();
        var successful = random.NextDouble() < 0.5;

        var newStatus = successful ? InvoiceDeliveryStatus.Successful : InvoiceDeliveryStatus.Failed;
        
        await repositoryManager.InvoiceDeliveries.MarkAsStatusAsync(invoiceDelivery, newStatus);
        await repositoryManager.SaveChangesAsync();
        
        // await invoiceDeliveryService.MarkAsStatusAsync(invoiceDelivery, newStatus);
    }
}
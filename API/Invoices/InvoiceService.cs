using Models.InvoiceDeliveries;
using Models.Invoices;
using Repositories;

namespace API.Invoices;

public class InvoiceService(IRepositoryManager repositoryManager)
{
    public async Task CreateInvoice(InvoiceTemplate invoiceTemplate)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = invoiceTemplate.UserId,
            Price = invoiceTemplate.Price,
            Description = invoiceTemplate.Description,
            DateCreated = today,
        };

        var invoiceDelivery = new InvoiceDelivery
        {
            InvoiceId = invoice.Id,
            InvoiceTemplateId = invoiceTemplate.Id,
            ScheduleId = invoiceTemplate.ScheduleId,
            Status = InvoiceDeliveryStatus.InProgress,
            DateCreated = today,
        };
        
        await repositoryManager.Invoices.CreateAsync(invoice);
        await repositoryManager.InvoiceDeliveries.CreateAsync(invoiceDelivery);
        await repositoryManager.SaveChangesAsync();
    }
}
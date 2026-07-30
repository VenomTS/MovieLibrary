using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using Repositories;

namespace API.InvoiceTemplates;

public class InvoiceTemplateService(IRepositoryManager repositoryManager)
{
    public async Task<List<InvoiceTemplate>> GetScheduledInvoicesAsync(DateOnly date)
    {
        // Ovo se koristi samo za nove invoices, stari (failed) invoices su handle-ovani u InvoiceDeliveryService
        var scheduledInvoices = await repositoryManager.InvoiceTemplates.AsQueryable()
            .Include(x => x.Schedule)
            .Where(invoiceTemplate => invoiceTemplate.Schedule.NextOccurrence <= date &&
                                      !repositoryManager.InvoiceDeliveries.AsQueryable()
                                          .Any(invoiceDelivery =>
                                              invoiceDelivery.InvoiceTemplateId == invoiceTemplate.Id &&
                                              invoiceDelivery.DateCreated == date &&
                                              invoiceDelivery.ScheduleId == invoiceTemplate.ScheduleId))
            .ToListAsync();
        return scheduledInvoices;
    }
}
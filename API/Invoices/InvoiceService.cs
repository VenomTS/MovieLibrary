using API.InvoiceCounters;
using Models.InvoiceDeliveries;
using Models.Invoices;
using Repositories;

namespace API.Invoices;

public class InvoiceService(IRepositoryManager repositoryManager, InvoiceCounterService invoiceCounterService)
{
    private const string NumberFormat = "D12";
    
    public async Task CreateAutomaticInvoice(InvoiceTemplate invoiceTemplate)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        await using var transaction = await repositoryManager.BeginTransactionAsync();

        try
        {
            var invoiceNumber = await invoiceCounterService.GetAndIncrementCountByYear(today.Year);
            
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = invoiceTemplate.UserId,
                Price = invoiceTemplate.Price,
                Description = invoiceTemplate.Description,
                DateCreated = today,
                Number = invoiceNumber.ToString(NumberFormat),
            };

            var invoiceDelivery = new InvoiceDelivery
            {
                InvoiceId = invoice.Id,
                InvoiceTemplateId = invoiceTemplate.Id,
                ScheduleId = invoiceTemplate.ScheduleId,
                DeliveryStatus = InvoiceDeliveryStatus.InProgress,
                DateCreated = today,
            };
        
            await repositoryManager.Invoices.CreateAsync(invoice);
            await repositoryManager.InvoiceDeliveries.CreateAsync(invoiceDelivery);
        
            await repositoryManager.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
        }
    }
}
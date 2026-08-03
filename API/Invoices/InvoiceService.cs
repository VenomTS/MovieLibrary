using Models.InvoiceDeliveries;
using Models.Invoices;
using Repositories;

namespace API.Invoices;

public class InvoiceService(IRepositoryManager repositoryManager)
{
    private const string NumberFormat = "D6";
    private const int MaxValue = 999999;
    private const string StartingInvoiceNumber = "0";
    
    public async Task CreateAutomaticInvoice(InvoiceTemplate invoiceTemplate)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        await using var transaction = await repositoryManager.BeginTransactionAsync();

        try
        {
            var invoiceNumber = await GetNextInvoiceNumber();
            
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

    private async Task<int> GetNextInvoiceNumber()
    {
        var highestInvoiceNumberStr = await repositoryManager.Invoices.GetMaxNumber(/*Optionally add Skladiste*/);

        // Just to be safe
        if (string.IsNullOrEmpty(highestInvoiceNumberStr) || string.IsNullOrWhiteSpace(highestInvoiceNumberStr))
            highestInvoiceNumberStr = StartingInvoiceNumber;
            
        var invoiceNumber = int.TryParse(highestInvoiceNumberStr, out var result) ? result + 1 : MaxValue;
        return invoiceNumber;
    }
}
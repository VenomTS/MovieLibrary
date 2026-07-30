using Microsoft.EntityFrameworkCore;
using Models.InvoiceDeliveries;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceDeliveryRepository(AppDbContext dbContext) : RepositoryBase<InvoiceDelivery>(dbContext), IInvoiceDeliveryRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task QueueUnsuccessfulInvoicesAsync()
    {
        await _dbContext.InvoiceDeliveries
            .Where(x => x.DeliveryStatus != InvoiceDeliveryStatus.Successful)
            .ExecuteUpdateAsync(x => x.SetProperty(id => id.DeliveryStatus, InvoiceDeliveryStatus.InProgress));
    }

    public async Task MarkAsStatusAsync(InvoiceDelivery invoiceDelivery, InvoiceDeliveryStatus status)
    {
        await _dbContext.InvoiceDeliveries
            .Where(x => x.InvoiceId == invoiceDelivery.InvoiceId && x.DateCreated == invoiceDelivery.DateCreated)
            .ExecuteUpdateAsync(x => x.SetProperty(id => id.DeliveryStatus, status));
    }

    public async Task<List<InvoiceDelivery>> GetInProgressInvoicesAsync()
    {
        return await _dbContext.InvoiceDeliveries
            .Include(x => x.Invoice)
            .Where(x => x.DeliveryStatus == InvoiceDeliveryStatus.InProgress)
            .ToListAsync();
    }
}
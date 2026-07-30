using Models.InvoiceDeliveries;

namespace Repositories.Interfaces;

public interface IInvoiceDeliveryRepository : IRepositoryBase<InvoiceDelivery>
{
    public Task QueueUnsuccessfulInvoicesAsync();
    public Task MarkAsStatusAsync(InvoiceDelivery invoiceDelivery, InvoiceDeliveryStatus status);
    public Task<List<InvoiceDelivery>> GetInProgressInvoicesAsync();
}
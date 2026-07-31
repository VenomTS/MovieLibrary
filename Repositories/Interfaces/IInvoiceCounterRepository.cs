using Models.Invoices;

namespace Repositories.Interfaces;

public interface IInvoiceCounterRepository : IRepositoryBase<InvoiceCounter>
{
    public Task<InvoiceCounter?> GetByYearAsync(int year);
}
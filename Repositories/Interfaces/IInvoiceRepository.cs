using Models.Invoices;

namespace Repositories.Interfaces;

public interface IInvoiceRepository : IRepositoryBase<Invoice>
{
    Task<string> GetMaxNumber();
}
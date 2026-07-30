using DTO.InvoiceTemplates;
using Models.Invoices;

namespace Repositories.Interfaces;

public interface IInvoiceTemplateRepository : IRepositoryBase<InvoiceTemplate>
{
    Task<InvoiceTemplate?> GetByUserIdAsync(Guid userId);
}
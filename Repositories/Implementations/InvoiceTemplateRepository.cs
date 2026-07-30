using Models.Invoices;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceTemplateRepository(AppDbContext dbContext) : RepositoryBase<InvoiceTemplate>(dbContext), IInvoiceTemplateRepository
{
    
}
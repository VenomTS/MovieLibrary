using Models.InvoiceDeliveries;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceDeliveryRepository(AppDbContext dbContext) : RepositoryBase<InvoiceDelivery>(dbContext), IInvoiceDeliveryRepository
{
}
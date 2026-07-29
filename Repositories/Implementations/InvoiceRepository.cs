using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceRepository(AppDbContext dbContext) : RepositoryBase<Invoice>(dbContext), IInvoiceRepository
{
}
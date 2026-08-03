using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceRepository(AppDbContext dbContext) : RepositoryBase<Invoice>(dbContext), IInvoiceRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<string?> GetMaxNumber()
    {
        return await _dbContext.Invoices.MaxAsync(x => x.Number);
    }
}
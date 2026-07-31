using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceCounterRepository(AppDbContext dbContext) : RepositoryBase<InvoiceCounter>(dbContext), IInvoiceCounterRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<InvoiceCounter?> GetByYearAsync(int year)
    {
        return await _dbContext.InvoiceCounters.Where(x => x.Year == year).FirstOrDefaultAsync();
    }
}
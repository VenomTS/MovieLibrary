using DTO.InvoiceTemplates;
using Microsoft.EntityFrameworkCore;
using Models.Invoices;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceTemplateRepository(AppDbContext dbContext) : RepositoryBase<InvoiceTemplate>(dbContext), IInvoiceTemplateRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<InvoiceTemplate?> GetByUserIdAsync(Guid userId)
    {
        return await _dbContext.InvoiceTemplates
            .Where(x => x.UserId == userId)
            .Include(x => x.Schedule)
            .ThenInclude(x => x.RecurrenceRule)
            .FirstOrDefaultAsync();
    }
}
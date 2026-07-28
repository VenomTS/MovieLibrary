using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class InvoiceRepository(AppDbContext dbContext) : RepositoryBase<Invoice>(dbContext), IInvoiceRepository
{
    public async Task<Invoice?> GetLastInvoiceByScheduleId(Guid scheduleId)
    {
        return await dbContext.Invoices.Where(x => x.ScheduleId == scheduleId)
            .OrderByDescending(x => x.DateSent)
            .FirstOrDefaultAsync();
    }
}
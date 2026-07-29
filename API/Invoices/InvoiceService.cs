using Microsoft.EntityFrameworkCore;
using Models;
using Models.Schedules;
using Repositories;

namespace API.Invoices;

public class InvoiceService(IRepositoryManager repositoryManager)
{
    public async Task<List<Invoice>> GetScheduledInvoices(List<Guid> scheduleIds)
    {
        return await repositoryManager.Invoices.AsQueryable()
            .Where(x => scheduleIds.Contains(x.ScheduleId))
            .ToListAsync();
    }
}
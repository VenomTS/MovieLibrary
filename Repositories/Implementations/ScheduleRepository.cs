using Microsoft.EntityFrameworkCore;
using Models.Schedules;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class ScheduleRepository(AppDbContext dbContext) : RepositoryBase<Schedule>(dbContext), IScheduleRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<List<Schedule>> GetScheduledAsync(DateOnly date)
    {
        return await _dbContext.Schedules.Include(x => x.RecurrenceRule)
            .Where(x => x.NextOccurrence <= date)
            .ToListAsync();
    }
}
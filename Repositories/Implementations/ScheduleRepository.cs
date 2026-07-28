using Models.Schedules;
using Repositories.Database;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class ScheduleRepository(AppDbContext dbContext) : RepositoryBase<ScheduleBase>(dbContext), IScheduleRepository
{
}
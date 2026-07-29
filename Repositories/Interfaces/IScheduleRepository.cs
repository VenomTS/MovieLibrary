using Models.Schedules;

namespace Repositories.Interfaces;

public interface IScheduleRepository : IRepositoryBase<Schedule>
{
    public Task<List<Schedule>> GetScheduledAsync(DateOnly date);
}
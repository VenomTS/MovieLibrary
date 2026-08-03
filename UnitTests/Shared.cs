using Models;
using Models.Schedules;
using Models.Schedules.Rules;
using Repositories.Database;

namespace UnitTests;

public class Shared
{
    public static Schedule CreateSchedule(Frequency frequency, DateOnly startDate, int interval)
    {
        return new Schedule
        {
            StartDate = startDate,
            RecurrenceRule = new RecurrenceRule
            {
                Frequency = frequency,
                Interval = interval
            }
        };
    }

    public static async Task<AppUser> CreateUser(AppDbContext context)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }
}
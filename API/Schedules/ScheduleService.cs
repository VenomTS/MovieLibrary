using DTO.Schedules;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Schedules;
using Repositories;

namespace API.Schedules;

public class ScheduleService(IRepositoryManager repositoryManager)
{

    private const DayOfWeek WeekStartsWith = DayOfWeek.Monday;
    private const int WeekLength = 7;
    
    public async Task<List<ScheduleBase>> GetScheduledSchedulesAsync()
    {
        // Ideja: Dodati LastTrigger na Schedule, tako da uvijek kad posaljem invoice znam kad sam ga poslao
        var today = DateOnly.FromDateTime(DateTime.Now);
        var schedules = await repositoryManager.Schedules.AsQueryable()
            .Where(x => x.StartDate <= today && (x.EndDate == null || x.EndDate >= today))
            .ToListAsync();
        
        var scheduledSchedules = new List<ScheduleBase>();

        foreach (var schedule in schedules)
        {
            switch (schedule)
            {
                case DailySchedule dailySchedule when IsScheduled(dailySchedule):
                case WeeklySchedule weeklySchedule when IsScheduled(weeklySchedule):
                case MonthlySchedule monthlySchedule when IsScheduled(monthlySchedule):
                    scheduledSchedules.Add(schedule);
                    break;
            }
        }

        return scheduledSchedules;
    }

    private static bool IsScheduled(DailySchedule schedule)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        var enoughTimePassed = today >= schedule.LastTrigger.AddDays(schedule.IntervalDays);

        return enoughTimePassed && (!schedule.OnlyWeekdays || IsWeekDay(today.DayOfWeek));
    }
    
    private static bool IsScheduled(WeeklySchedule schedule)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        var nextInvoiceWeek = schedule.LastTrigger.AddDays(WeekLength * schedule.IntervalWeeks);
        var daysSinceWeekStart = ((int) nextInvoiceWeek.DayOfWeek - (int) WeekStartsWith + 7) % 7;
        var weekStartDate = nextInvoiceWeek.AddDays(-daysSinceWeekStart);

        var enoughTimePassed = schedule.LastTrigger < weekStartDate && weekStartDate <= today;

        return enoughTimePassed && IsDayMatch(schedule.ScheduleDays, today.DayOfWeek);
    }
    
    private static bool IsScheduled(MonthlySchedule schedule)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        var enoughTimePassed =
            (today.Year - schedule.LastTrigger.Year) * 12 + 
            (today.Month - schedule.LastTrigger.Month) >=
            schedule.IntervalMonths;

        if (!enoughTimePassed)
            return false;
        
        if(schedule.IntervalDays != null)
            return schedule.IntervalDays.Value == today.Day;

        // return schedule.Day switch
        // {
        //     MonthlyDay.Day => true,
        //     MonthlyDay.WeekDay => IsWeekDay(today.DayOfWeek),
        //     MonthlyDay.WeekEndDay => !IsWeekDay(today.DayOfWeek),
        //     _ => schedule.Day != null && IsDayMatch(schedule.Day.Value, today.DayOfWeek)
        // };
    }

    private static bool IsWeekDay(DayOfWeek dayOfWeek)
    {
        return dayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
    }

    private static bool IsDayMatch(ScheduleDays days, DayOfWeek dayOfWeek)
    {
        var convertedDayOfWeek = ((int) dayOfWeek + 6) % 7;
        var flag = (ScheduleDays) (1 << convertedDayOfWeek);
        return days.HasFlag(flag);
    }

    private static bool IsDayMatch(MonthlyDay day, DayOfWeek dayOfWeek)
    {
        var convertedDayOfWeek = ((int) dayOfWeek + 6) % 7;
        return (int) day == convertedDayOfWeek;
    }
    
    public async Task<List<ScheduleBase>> GetAllAsync()
    {
        var schedules = await repositoryManager.Schedules.GetAllAsync();

        foreach (var sch in schedules)
        {
            if (sch is DailySchedule daily)
            {
                Console.WriteLine("Found daily");
            }

            if (sch is WeeklySchedule weekly)
            {
                Console.WriteLine("Found weekly");
            }

            if (sch is MonthlySchedule monthly)
            {
                Console.WriteLine("Found monthly");
            }
        }

        return [];
    }

    public async Task CreateAsync(CreateScheduleRequest request)
    {
        // Apply checks bla bla bla
        if (request.Type == CreateScheduleTypes.Daily)
        {
            var daily = new DailySchedule
            {
                UserId = request.UserId,
                Time = request.Time,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IntervalDays = request.IntervalDays!.Value,
                OnlyWeekdays = request.EveryWeekday!.Value,
                LastTrigger = DateOnly.MinValue,
            };
            
            await repositoryManager.Schedules.CreateAsync(daily);
        }
        else if (request.Type == CreateScheduleTypes.Weekly)
        {
            var weekly = new WeeklySchedule
            {
                UserId = request.UserId,
                Time = request.Time,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IntervalWeeks = request.IntervalWeeks!.Value,
                ScheduleDays = request.ScheduleDays!.Value,
                LastTrigger = DateOnly.MinValue,
            };
            await repositoryManager.Schedules.CreateAsync(weekly);
        }
        else
        {
            var monthly = new MonthlySchedule
            {
                UserId = request.UserId,
                Time = request.Time,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IntervalMonths = request.IntervalMonths!.Value,
                DayType = request.DayType,
                Day = request.Day,
                LastTrigger = DateOnly.MinValue,
            };
            await repositoryManager.Schedules.CreateAsync(monthly);
        }
        
        await repositoryManager.SaveChangesAsync();
    }
}
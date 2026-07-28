using DTO.Schedules;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Schedules;
using Repositories;

namespace API.Schedules;

public class ScheduleService(IRepositoryManager repositoryManager)
{

    private const DayOfWeek WeekStartsWith = DayOfWeek.Monday;
    
    public async Task<List<ScheduleBase>> GetScheduledSchedulesAsync()
    {
        // Ideja: Dodati LastSent na Schedule, tako da uvijek kad posaljem invoice znam kad sam ga poslao
        var today = DateOnly.FromDateTime(DateTime.Now);
        var schedules = await repositoryManager.Schedules.AsQueryable()
            .Where(x => x.StartDate <= today && (x.EndDate == null || x.EndDate >= today))
            .Select(x => new
            {
                Schedule = x,
                LastSent = repositoryManager.Invoices.AsQueryable()
                    .Where(y => y.ScheduleId == x.Id)
                    .OrderByDescending(y => y.DateSent)
                    .FirstOrDefault()
            }).ToListAsync();
        
        var scheduledSchedules = new List<ScheduleBase>();

        foreach (var schedule in schedules)
        {
            switch (schedule.Schedule)
            {
                case DailySchedule dailySchedule when IsScheduled(dailySchedule, schedule.LastSent):
                case WeeklySchedule weeklySchedule when IsScheduled(weeklySchedule, schedule.LastSent):
                case MonthlySchedule monthlySchedule when IsScheduled(monthlySchedule, schedule.LastSent):
                    scheduledSchedules.Add(schedule.Schedule);
                    break;
            }
        }

        return scheduledSchedules;
    }

    private static bool IsScheduled(DailySchedule schedule, Invoice? lastSentInvoice)
    {
        // WORKS????
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        // Ako nema prethodnog invoice, onda saljemo danas
        if (lastSentInvoice == null)
            return (schedule.OnlyWeekdays && IsWeekDay(today.DayOfWeek)) || !schedule.OnlyWeekdays;
        
        // Ako ima prethodnog invoice, moramo provjeriti da li je interval pravilan
        var enoughTimePassed = today > lastSentInvoice.DateSent.AddDays(schedule.IntervalDays);

        return enoughTimePassed && (!schedule.OnlyWeekdays || (schedule.OnlyWeekdays && IsWeekDay(today.DayOfWeek)));
    }
    
    private static bool IsScheduled(WeeklySchedule schedule, Invoice? lastSentInvoice)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        if (lastSentInvoice == null)
            return IsDayMatch(schedule.ScheduleDays, today.DayOfWeek);
        
        var daysSinceMonday = ((int) today.DayOfWeek - (int) WeekStartsWith + 7) % 7;
        var mondayDate = today.AddDays(-daysSinceMonday);
        
        // Ako je prethodni invoice poslat prije najblizeg ponedjeljka
        // i danas je ponedjeljak ili kasnije
        // i dan odgovara
        // Onda je valid
        return lastSentInvoice.DateSent < mondayDate && mondayDate <= today && IsDayMatch(schedule.ScheduleDays, today.DayOfWeek);
    }
    
    private bool IsScheduled(MonthlySchedule schedule, Invoice? lastSentInvoice)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var passedLongEnough = lastSentInvoice == null || today.Month > lastSentInvoice.DateSent.AddMonths(schedule.IntervalMonths).Month;

        return false;
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
                OnlyWeekdays = request.EveryWeekday!.Value
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
            };
            await repositoryManager.Schedules.CreateAsync(monthly);
        }
        
        await repositoryManager.SaveChangesAsync();
    }
}
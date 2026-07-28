using DTO.Schedules;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Schedules;
using Repositories;

namespace API.Schedules;

public class ScheduleService(IRepositoryManager repositoryManager)
{
    public async Task<List<ScheduleBase>> GetScheduledSchedulesAsync()
    {
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
            return (schedule.EveryWeekday && IsWeekDay(today.DayOfWeek)) || !schedule.EveryWeekday;
        
        // Ako ima prethodnog invoice, moramo provjeriti da li je interval pravilan
        if(schedule.EveryWeekday)
            return IsWeekDay(today.DayOfWeek);

        if (schedule.IntervalDays == null)
            throw new Exception("Interval days not set");
        
        return today >= lastSentInvoice.DateSent.AddDays(schedule.IntervalDays!.Value);
    }
    
    private bool IsScheduled(WeeklySchedule schedule, Invoice? lastSentInvoice)
    {
        return false;
    }
    
    private bool IsScheduled(MonthlySchedule schedule, Invoice? lastSentInvoice)
    {
        return false;
    }

    private static bool IsWeekDay(DayOfWeek dayOfWeek)
    {
        return dayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);
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
                IntervalDays = request.IntervalDays,
                EveryWeekday = request.EveryWeekday!.Value
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
using Microsoft.EntityFrameworkCore;
using Models.Schedules;
using Models.Schedules.Rules;
using Repositories;

namespace API.Schedules;

public class ScheduleService(IRepositoryManager repositoryManager)
{
    private const DayOfWeek WeekStartsWith = DayOfWeek.Monday;
    private const int WeekLength = 7;

    public async Task UpdateNextOccurrenceAsync(Schedule schedule)
    {
        await repositoryManager.Schedules.Update(schedule);
        
        schedule.NextOccurrence = GetNextOccurrence(schedule, DateOnly.FromDateTime(DateTime.Now));
        await repositoryManager.SaveChangesAsync();
    }

    public static DateOnly GetNextOccurrence(Schedule schedule, DateOnly date)
    {
        // var today = DateOnly.FromDateTime(DateTime.Now);
        var startDate = schedule.StartDate > date ? schedule.StartDate : date;

        return schedule.RecurrenceRule.Frequency switch
        {
            Frequency.Daily => GetDailyOccurrence(schedule.RecurrenceRule, startDate),
            Frequency.Weekly => GetWeeklyOccurrence(schedule.RecurrenceRule, startDate),
            Frequency.Monthly => GetMonthlyOccurrence(schedule.RecurrenceRule, startDate),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static DateOnly GetDailyOccurrence(RecurrenceRule rule, DateOnly startDate)
    {
        if (rule.DaysOfWeek == null) return startDate.AddDays(rule.Interval);
        
        var candidate = startDate.AddDays(1);
        while(candidate.DayOfWeek is (DayOfWeek.Saturday or DayOfWeek.Sunday))
            candidate = candidate.AddDays(1);

        return candidate;
    }
    
    private static DateOnly GetWeeklyOccurrence(RecurrenceRule rule, DateOnly startDate)
    {
        var candidate = startDate.AddDays(1);

        while (true)
        {
            var weekStart = GetWeekStart(candidate);

            var weeksSinceStart =
                (weekStart.DayNumber - GetWeekStart(startDate).DayNumber) / 7;

            var isValidWeek = weeksSinceStart % rule.Interval == 0;

            if (isValidWeek)
            {
                while (candidate < weekStart.AddDays(7))
                {
                    if (IsDaySelected(rule.DaysOfWeek!.Value, candidate.DayOfWeek))
                        return candidate;

                    candidate = candidate.AddDays(1);
                }
            }
            else
            {
                candidate = weekStart.AddDays(7);
            }
        }
    }

    private static DateOnly GetWeekStart(DateOnly date)
    {
        var difference = ((int) date.DayOfWeek - (int) WeekStartsWith + 7) % 7;
        return date.AddDays(-difference);
    }
    
    private static bool IsDaySelected(DaysOfWeek days, DayOfWeek day)
    {
        var flag = day switch
        {
            DayOfWeek.Monday => DaysOfWeek.Monday,
            DayOfWeek.Tuesday => DaysOfWeek.Tuesday,
            DayOfWeek.Wednesday => DaysOfWeek.Wednesday,
            DayOfWeek.Thursday => DaysOfWeek.Thursday,
            DayOfWeek.Friday => DaysOfWeek.Friday,
            DayOfWeek.Saturday => DaysOfWeek.Saturday,
            DayOfWeek.Sunday => DaysOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException()
        };

        return (days & flag) != 0;
    }

    private static DateOnly GetMonthlyOccurrence(RecurrenceRule rule, DateOnly startDate)
    {
        var candidateMonth = startDate.AddMonths(rule.Interval);
        
        if (rule.DayOfMonth != null)
        {
            var candidateDay = Math.Min(rule.DayOfMonth!.Value,
                DateTime.DaysInMonth(candidateMonth.Year, candidateMonth.Month));
            return new DateOnly(candidateMonth.Year, candidateMonth.Month, candidateDay);
        }
        
        return GetDateFromOrdinalOccurence(candidateMonth, rule.Ordinal!.Value, rule.OrdinalType!.Value);
    }

    private static DateOnly GetDateFromOrdinalOccurence(DateOnly month, Ordinal ordinal, OrdinalType ordinalType)
    {
        var days = GetDaysForOrdinalType(ordinalType);

        if (ordinal == Ordinal.Last)
        {
            var lastDay = new DateOnly(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));

            while (!days.Contains(lastDay.DayOfWeek))
                lastDay = lastDay.AddDays(-1);

            return lastDay;
        }

        var occurenceNumber = ordinal switch
        {
            Ordinal.First => 1,
            Ordinal.Second => 2,
            Ordinal.Third => 3,
            Ordinal.Fourth => 4,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var candidate = new DateOnly(month.Year, month.Month, 1);
        var count = 0;

        while (candidate.Month == month.Month)
        {
            if (days.Contains(candidate.DayOfWeek))
            {
                count += 1;
                if (count == occurenceNumber)
                    return candidate;
                
                // Dan pronadjen, dodaji sedmicu
                candidate = candidate.AddDays(WeekLength);
                continue;
            }
            
            candidate = candidate.AddDays(1);
        }

        throw new InvalidOperationException(
            $"Could not find {ordinal} {ordinalType} in {month:yyyy-MM}");
    }
    
    private static HashSet<DayOfWeek> GetDaysForOrdinalType(OrdinalType type)
    {
        return type switch
        {
            OrdinalType.Monday => [DayOfWeek.Monday],

            OrdinalType.Tuesday => [DayOfWeek.Tuesday],

            OrdinalType.Wednesday => [DayOfWeek.Wednesday],

            OrdinalType.Thursday => [DayOfWeek.Thursday],

            OrdinalType.Friday => [DayOfWeek.Friday],

            OrdinalType.Saturday => [DayOfWeek.Saturday],

            OrdinalType.Sunday => [DayOfWeek.Sunday],

            OrdinalType.Day => Enum.GetValues<DayOfWeek>().ToHashSet(),

            OrdinalType.WeekDay =>
            [
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday
            ],

            OrdinalType.WeekEndDay =>
            [
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            ],

            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
    
    public async Task<Schedule> FindOrCreateSchedule(Schedule schedule)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        
        // Merge if: Oba u proslosti ILI Oba imaju isti dan; 
        
        var existingSchedule = await repositoryManager.Schedules.AsQueryable()
            .Where(x => ((x.StartDate <= today && schedule.StartDate <= today) || x.StartDate == schedule.StartDate) &&
                        x.EndDate == schedule.EndDate &&
                        x.RecurrenceRule.Frequency == schedule.RecurrenceRule.Frequency &&
                        x.RecurrenceRule.Interval == schedule.RecurrenceRule.Interval &&
                        x.RecurrenceRule.DaysOfWeek == schedule.RecurrenceRule.DaysOfWeek &&
                        x.RecurrenceRule.DayOfMonth == schedule.RecurrenceRule.DayOfMonth &&
                        x.RecurrenceRule.Ordinal == schedule.RecurrenceRule.Ordinal &&
                        x.RecurrenceRule.OrdinalType == schedule.RecurrenceRule.OrdinalType)
            .FirstOrDefaultAsync();

        if (existingSchedule != null)
            return existingSchedule;

        return await CreateAsync(schedule);
    }

    public async Task<Schedule> CreateAsync(Schedule schedule)
    {
        schedule.NextOccurrence = GetNextOccurrence(schedule, DateOnly.FromDateTime(DateTime.Now));
        await repositoryManager.Schedules.CreateAsync(schedule);
        await repositoryManager.SaveChangesAsync();
        return schedule;
    }
}
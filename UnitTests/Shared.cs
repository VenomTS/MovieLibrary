using Models.Schedules;
using Models.Schedules.Rules;

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
}
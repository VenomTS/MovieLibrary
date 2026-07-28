namespace Models.Schedules;

public class MonthlySchedule : ScheduleBase
{
    public int? IntervalDays { get; set; }
    public int IntervalMonths { get; set; }
    public MonthlyDayType? DayType { get; set; }
    public DayOfWeek? Day { get; set; }
}
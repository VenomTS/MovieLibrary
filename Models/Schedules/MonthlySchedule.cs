namespace Models.Schedules;

public class MonthlySchedule : ScheduleBase
{
    public int? IntervalDays { get; set; }
    public int IntervalMonths { get; set; }
    public MonthlyDayType? DayType { get; set; }
    // Dodati umjesto Day sve dane (Monday, Tuesday, ...) i EVERY DAY, EVERY WEEKDAY and EVERY WEEKEND DAY
    public DayOfWeek? Day { get; set; }
}
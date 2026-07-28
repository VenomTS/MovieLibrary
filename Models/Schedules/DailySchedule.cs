namespace Models.Schedules;

public class DailySchedule : ScheduleBase
{
    public int? IntervalDays { get; set; }
    public bool EveryWeekday { get; set; }
}
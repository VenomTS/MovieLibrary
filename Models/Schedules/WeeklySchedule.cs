namespace Models.Schedules;

public class WeeklySchedule : ScheduleBase
{
    public int IntervalWeeks { get; set; }
    public ScheduleDays ScheduleDays { get; set; }
}
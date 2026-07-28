using Models.Schedules;

namespace DTO.Schedules;

public class CreateScheduleRequest
{
    public Guid UserId { get; set; }
    public TimeOnly Time { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public CreateScheduleTypes Type { get; set; }
    
    // Daily Schedule
    public int? IntervalDays { get; set; }
    public bool? EveryWeekday { get; set; }
    
    // Weekly Schedule
    public int? IntervalWeeks { get; set; }
    public ScheduleDays? ScheduleDays { get; set; }
    
    // Monthly Schedule
    public int? IntervalMonths { get; set; }
    public MonthlyDayType? DayType { get; set; }
    public DayOfWeek? Day { get; set; }
}
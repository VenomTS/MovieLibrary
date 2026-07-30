using Models.Schedules.Rules;

namespace DTO.Schedules;

public class ScheduleResponse
{
    public Guid Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly NextOccurrence { get; set; }
    public Frequency Frequency { get; set; }
    public int Interval { get; set; }
    public DaysOfWeek? DaysOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public Ordinal? Ordinal { get; set; }
    public OrdinalType? OrdinalType { get; set; }
}
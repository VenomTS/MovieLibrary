using Models.Schedules.Rules;

namespace DTO.Schedules;

public class CreateRecurrenceRuleRequest
{
    public Frequency Frequency { get; set; }
    public int Interval { get; set; }
    public DaysOfWeek? DaysOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public Ordinal? Ordinal { get; set; }
    public OrdinalType? OrdinalType { get; set; }
    public TimeOnly Period { get; set; }
}
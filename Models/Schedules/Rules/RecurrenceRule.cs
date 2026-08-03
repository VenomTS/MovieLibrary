using Microsoft.EntityFrameworkCore;

namespace Models.Schedules.Rules;

[Owned]
public class RecurrenceRule
{
    public Frequency Frequency { get; set; }
    public int Interval { get; set; }
    public DaysOfWeek? DaysOfWeek { get; set; }
    public int? DayOfMonth { get; set; }
    public Ordinal? Ordinal { get; set; }
    public OrdinalType? OrdinalType { get; set; }
}
using Models.Schedules.Rules;

namespace Models.Schedules;

public class Schedule
{
    public Guid Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly NextOccurrence { get; set; }
    public RecurrenceRule RecurrenceRule { get; set; }
}
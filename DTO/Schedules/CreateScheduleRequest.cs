namespace DTO.Schedules;

public class CreateScheduleRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public CreateRecurrenceRuleRequest RecurrenceRule { get; set; }
}
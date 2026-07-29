namespace Models.Schedules;

public abstract class ScheduleBase
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public TimeOnly Time { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateOnly LastTrigger { get; set; }

    public AppUser AppUser { get; set; }
}
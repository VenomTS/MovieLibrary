using Models.Schedules;

namespace Models;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;

    public AppUser AppUser { get; set; }
    public ScheduleBase Schedule { get; set; }
}
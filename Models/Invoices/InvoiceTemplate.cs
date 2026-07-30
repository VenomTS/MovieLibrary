using Models.Schedules;

namespace Models.Invoices;

public class InvoiceTemplate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;

    public AppUser User { get; set; }
    public Schedule Schedule { get; set; }
}
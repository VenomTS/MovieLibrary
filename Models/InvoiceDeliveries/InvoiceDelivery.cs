using Models.Schedules;

namespace Models.InvoiceDeliveries;

public class InvoiceDelivery
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid ScheduleId { get; set; }
    public InvoiceDeliveryStatus Status { get; set; }
    
    public DateOnly AttemptedAt { get; set; }
    
    public DateOnly OriginalDate { get; set; }

    public Invoice Invoice { get; set; }
    public Schedule Schedule { get; set; }
}
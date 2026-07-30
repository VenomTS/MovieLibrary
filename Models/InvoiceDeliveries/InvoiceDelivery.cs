using Models.Invoices;

namespace Models.InvoiceDeliveries;

public class InvoiceDelivery
{
    // Composite Key: InvoiceTemplateId + ScheduleId + DateCreated
    
    public Guid InvoiceTemplateId { get; set; }
    public Guid ScheduleId { get; set; }
    
    public Guid InvoiceId { get; set; }
    public InvoiceDeliveryStatus Status { get; set; }
    
    public DateOnly DateCreated { get; set; }

    public Invoice Invoice { get; set; }
}
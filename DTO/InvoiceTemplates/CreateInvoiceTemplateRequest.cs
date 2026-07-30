namespace DTO.InvoiceTemplates;

public class CreateInvoiceTemplateRequest
{
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
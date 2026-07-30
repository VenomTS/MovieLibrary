namespace DTO.InvoiceTemplates;

public class InvoiceTemplateResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ScheduleId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
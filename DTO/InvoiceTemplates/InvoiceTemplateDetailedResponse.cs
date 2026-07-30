using DTO.Schedules;

namespace DTO.InvoiceTemplates;

public class InvoiceTemplateDetailedResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ScheduleResponse Schedule { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
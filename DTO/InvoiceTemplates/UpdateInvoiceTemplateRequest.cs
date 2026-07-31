namespace DTO.InvoiceTemplates;

public class UpdateInvoiceTemplateRequest
{
    public CreateInvoiceTemplateScheduleRequest Schedule { get; set; }
    
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
}
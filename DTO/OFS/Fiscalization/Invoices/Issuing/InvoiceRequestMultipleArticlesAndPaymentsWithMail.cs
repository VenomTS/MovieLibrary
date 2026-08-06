namespace DTO.OFS.Fiscalization.Invoices.Issuing;

public class InvoiceRequestMultipleArticlesAndPaymentsWithMail
{
    public bool Print { get; set; }
    public string Email { get; set; }
    public InvoiceRequest InvoiceRequest { get; set; }
}
namespace DTO.OFS.Fiscalization.Invoices.Issuing;

public class AdvanceInvoice
{
    public InvoiceRequest InvoiceRequest { get; set; }
    public List<PaymentRequest> AdvancePayment { get; set; }
}
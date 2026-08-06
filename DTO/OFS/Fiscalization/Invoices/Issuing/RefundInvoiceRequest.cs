namespace DTO.OFS.Fiscalization.Invoices.Issuing;

public class RefundInvoiceRequest : InvoiceRequest
{
    public string ReferentDocumentNumber { get; set; }
    public DateTimeOffset ReferentDocumentDT { get; set; }
}
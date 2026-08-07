using DTO.OFS.Fiscalization.InvoiceIssue;

namespace DTO.OFS.Mappers;

public static class InvoiceRequestMapper
{
    public static InvoiceRequest SetCashInvoice(this InvoiceRequest source)
    {
        source.InvoiceType = "Normal";
        source.TransactionType = "Sale";
        return source;
    }

    public static InvoiceRequest SetCopyInvoice(this InvoiceRequest source, string referentDocumentNumber,
        DateTimeOffset referentDocumentDT)
    {
        source.InvoiceType = "Copy";
        source.TransactionType = "Sale";
        source.ReferentDocumentNumber = referentDocumentNumber;
        source.ReferentDocumentDT = referentDocumentDT;
        return source;
    }

    public static InvoiceRequest SetRefundInvoice(this InvoiceRequest source, string referentDocumentNumber,
        DateTimeOffset referentDocumentDT)
    {
        source.InvoiceType = "Normal";
        source.TransactionType = "Refund";
        source.ReferentDocumentNumber = referentDocumentNumber;
        source.ReferentDocumentDT = referentDocumentDT;
        return source;
    }
}
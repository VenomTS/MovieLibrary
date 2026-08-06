namespace DTO.OFS.Fiscalization.InvoiceIssue;

public class InvoiceIssueResponse
{
    public string Address { get; set; }
    public string BusinessName { get; set; }
    public string District { get; set; }
    public string EncryptedInternalData { get; set; }
    public string InvoiceCounter { get; set; }
    public string InvoiceCounterExtension { get; set; }
    public string? InvoiceImageHtml { get; set; }
    public string? InvoiceImagePdfBase64 { get; set; }
    public string? InvoiceImagePngBase64 { get; set; }
    public string InvoiceNumber { get; set; }
    public string Journal { get; set; }
    public string LocationName { get; set; }
    public string Messages { get; set; }
    public string Mrc { get; set; }
    public string RequestedBy { get; set; }
    public DateTimeOffset SdcDateTime { get; set; }
    public string Signature { get; set; }
    public string SignedBy { get; set; }
    public int TaxGroupRevision { get; set; }
    public List<TaxItemResponse> TaxItems { get; set; }
    public string Tin { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalCounter { get; set; }
    public int TransactionTypeCounter { get; set; }
    public string VerificationQRCode { get; set; }
    public string VerificationUrl { get; set; }
}
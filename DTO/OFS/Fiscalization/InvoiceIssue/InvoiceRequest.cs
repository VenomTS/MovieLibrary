namespace DTO.OFS.Fiscalization.InvoiceIssue;

public class InvoiceRequest
{
    public string InvoiceType { get; set; }
    public string TransactionType { get; set; }
    public List<PaymentRequest> Payment { get; set; }
    public DateTimeOffset? DateAndTimeOfIssue { get; set; }
    public string? Cashier { get; set; }
    public string? BuyerId { get; set; }
    public string? BuyerCostCenterId { get; set; }
    public string? ReferentDocumentNumber { get; set; }
    public DateTimeOffset? ReferentDocumentDT { get; set; }
    public List<ItemRequest> Items { get; set; } = [];
}
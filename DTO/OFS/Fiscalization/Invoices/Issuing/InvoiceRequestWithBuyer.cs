namespace DTO.OFS.Fiscalization.Invoices.Issuing;

public class InvoiceRequestWithBuyer : InvoiceRequest
{
    public string BuyerId { get; set; }
    public string? BuyerCostCenterId { get; set; }
}
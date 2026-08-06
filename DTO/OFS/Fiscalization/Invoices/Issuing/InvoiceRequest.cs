namespace DTO.OFS.Fiscalization.Invoices.Issuing;

public class InvoiceRequest
{
    public string InvoiceType { get; set; }
    public string TransactionType { get; set; }
    public List<PaymentRequest> Payment { get; set; }
    public List<ItemRequest> Items { get; set; }
    public string Cashier { get; set; }
}
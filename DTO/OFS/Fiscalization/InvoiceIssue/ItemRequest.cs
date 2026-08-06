namespace DTO.OFS.Fiscalization.InvoiceIssue;

public class ItemRequest
{
    public string Name { get; set; }
    public string Gtin { get; set; }
    public List<string> Labels { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal DiscountAmount { get; set; }
}
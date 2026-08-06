namespace DTO.OFS.Fiscalization.Invoices.Issuing;

public class ItemRequest
{
    public string Name { get; set; }
    public string Gtin { get; set; }
    public List<string> Labels { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal UnitPrice { get; set; }
    public float Quantity { get; set; }
}
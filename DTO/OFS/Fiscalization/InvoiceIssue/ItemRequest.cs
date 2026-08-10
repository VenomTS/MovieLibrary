namespace DTO.OFS.Fiscalization.InvoiceIssue;

public class ItemRequest
{
    public string Name { get; set; } // Naziv
    public string Gtin { get; set; } // Sifra
    public List<string> Labels { get; set; } // TarifaSifra
    public decimal UnitPrice { get; set; } // MPC
    public decimal Quantity { get; set; } // Izlaz
    public decimal TotalAmount { get; set; } // Ovo se moze izracunat
    public decimal Discount { get; set; } // PopustPosto
    public decimal DiscountAmount { get; set; } // PopustVrijednost
}
namespace Models.Invoices;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly DateCreated { get; set; }

    public AppUser AppUser { get; set; }
}
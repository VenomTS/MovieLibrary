namespace DTO.OFS.Fiscalization.InvoiceIssue;

public class InvoiceIssueRequest
{
    public InvoiceRequest InvoiceRequest { get; set; }
    public bool Print { get; set; } = false;
    public string? Email { get; set; }
    public bool RenderReceiptImage { get; set; } = false;
    public string ReceiptImageFormat { get; set; } = "Png";
    public string ReceiptLayout { get; set; } = "Slip";
    public string? ReceiptHeaderImage { get; set; }
    public string? ReceiptFooterImage { get; set; }
    public List<string> ReceiptHeaderTextLines { get; set; } = [];
    public List<string> ReceiptFooterTextLines { get; set; } = [];
    public decimal AdvancePaid { get; set; }
    public decimal AdvanceTax { get; set; }
}
namespace DTO.OFS.Fiscalization.InvoiceIssue;

public class TaxItemResponse
{
    public decimal Amount { get; set; }
    public string CategoryName { get; set; }
    public int CategoryType { get; set; }
    public string Label { get; set; }
    public int Rate { get; set; }
}
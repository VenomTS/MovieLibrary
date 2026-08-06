namespace DTO.OFS.Fiscalization.Status;

public class TaxCategoryResponse
{
    public int CategoryType { get; set; }
    public string Name { get; set; }
    public int OrderId { get; set; }
    public List<TaxRateCategoryResponse> TaxRates { get; set; }
}